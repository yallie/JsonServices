using System.Collections.Concurrent;
using System.Linq;
using System.Security;
using JsonServices.Exceptions;
using SecureRemotePassword;

namespace JsonServices.Auth.SecureRemotePassword
{
	public class SrpAuthProvider : IAuthProvider
	{
		public SrpAuthProvider(ISrpAccountRepository repository, SrpParameters parameters = null)
		{
			AuthRepository = repository;
			SrpParameters = parameters ?? new SrpParameters();
			SrpServer = new SrpServer(SrpParameters);
			UnknownUserSalt = new SrpClient(parameters).GenerateSalt();
		}

		internal SrpServer SrpServer { get; set; }

		private ISrpAccountRepository AuthRepository { get; set; }

		private SrpParameters SrpParameters { get; set; }

		private string UnknownUserSalt { get; set; }

		// fixme: add a timeout to clean up pending data from step1 requests not followed by step2
		internal ConcurrentDictionary<string, Step1Data> PendingAuthentications { get; } =
			new ConcurrentDictionary<string, Step1Data>();

		// variables produced on the first authentication step
		internal class Step1Data
		{
			public ISrpAccount Account { get; set; }
			public string ClientEphemeralPublic { get; set; }
			public SrpEphemeral ServerEphemeral { get; set; }
		}

		public AuthResponse Authenticate(AuthRequest authRequest)
		{
			if (authRequest == null || authRequest.Parameters == null || !authRequest.Parameters.Any())
			{
				throw new AuthFailedException("No credentials specified");
			}

			var stepNumber = authRequest.GetStepNumber();
			if (!stepNumber.HasValue)
			{
				throw new AuthFailedException("Authentication protocol not supported: step number not specified");
			}

			var loginSession = authRequest.GetLoginSession();
			if (loginSession == null)
			{
				throw new AuthFailedException("Authentication protocol not supported: login session not specified");
			}

			// step number and session identity
			switch (stepNumber)
			{
				case 1:
					return AuthStep1(authRequest);

				case 2:
					return AuthStep2(authRequest);

				// step number should be either 1 or 2
				default:
					throw new AuthFailedException("Authentication protocol not supported: unknown step number");
			}
		}

		private AuthResponse AuthStep1(AuthRequest authRequest)
		{
			// first step never fails: User -> Host: I, A = g^a (identifies self, a = random number)
			var userName = authRequest.GetUserName();
			var clientEphemeralPublic = authRequest.GetClientPublicEphemeral();
			var account = AuthRepository.FindByName(userName);

			lock (SrpServer)
			{
				if (account != null)
				{
					// save the data for the second authentication step
					var salt = account.Salt;
					var verifier = account.Verifier;
					var serverEphemeral = SrpServer.GenerateEphemeral(verifier);
					PendingAuthentications[authRequest.GetLoginSession()] = new Step1Data
					{
						Account = account,
						ClientEphemeralPublic = clientEphemeralPublic,
						ServerEphemeral = serverEphemeral,
					};

					// Host -> User: s, B = kv + g^b (sends salt, b = random number)
					return ResponseStep1(salt, serverEphemeral.Public, authRequest.GetLoginSession());
				}

				var fakeSalt = SrpParameters.Hash(userName + UnknownUserSalt).ToHex();
				var fakeEphemeral = SrpServer.GenerateEphemeral(fakeSalt);
				return ResponseStep1(fakeSalt, fakeEphemeral.Public, authRequest.GetLoginSession());
			}
		}

		private AuthResponse AuthStep2(AuthRequest authRequest)
		{
			// get the values calculated on the first step
			Step1Data vars;
			if (!PendingAuthentications.TryRemove(authRequest.GetLoginSession(), out vars))
			{
				// avoid giving out too much information about the authentication failure
				throw new AuthFailedException(); // Authentication failed: retry the first step
			}

			try
			{
				// lock (SrpServer)
				{
					// second step may fail: User -> Host: M = H(H(N) xor H(g), H(I), s, A, B, K)
					var clientSessionProof = authRequest.GetClientSessionProof();
					var serverSession = SrpServer.DeriveSession(vars.ServerEphemeral.Secret,
						vars.ClientEphemeralPublic, vars.Account.Salt,
						vars.Account.UserName, vars.Account.Verifier, clientSessionProof);

					// Host -> User: H(A, M, K)
					return ResponseStep2(serverSession.Proof, vars.Account, authRequest.GetLoginSession());
				}
			}
			catch (SecurityException)
			{
				// avoid giving out too much information about the authentication failure
				throw new AuthFailedException(); // "Authentication failed: " + ex.Message
			}
		}

		private AuthResponse ResponseStep1(string salt, string serverPublicEphemeral, string loginSession)
		{
			var result = new AuthResponse();
			result.Parameters[SrpProtocolConstants.SaltKey] = salt;
			result.Parameters[SrpProtocolConstants.ServerPublicEphemeralKey] = serverPublicEphemeral;
			result.Parameters[SrpProtocolConstants.LoginSessionKey] = loginSession;
			return result;
		}

		private AuthResponse ResponseStep2(string serverSessionProof, ISrpAccount account, string loginSession)
		{
			var result = new AuthResponse();
			result.Parameters[SrpProtocolConstants.ServerSessionProofKey] = serverSessionProof;
			result.AuthenticatedIdentity = AuthRepository.GetIdentity(account);
			result.Parameters[SrpProtocolConstants.LoginSessionKey] = loginSession;
			return result;
		}
	}
}
