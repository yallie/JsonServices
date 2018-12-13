using System.Threading.Tasks;
using JsonServices.Exceptions;
using SecureRemotePassword;

namespace JsonServices.Auth.SecureRemotePassword
{
	public class SrpCredentials : CredentialsBase
	{
		public SrpCredentials(string userName, string password, SrpParameters parameters = null)
		{
			UserName = userName;
			Password = password;
			SrpClient = new SrpClient(parameters);
		}

		internal SrpClient SrpClient { get; set; }

		public override async Task Authenticate(JsonClient client)
		{
			// step1 request: User -> Host: I, A = g^a (identifies self, a = random number)
			var clientEphemeral = SrpClient.GenerateEphemeral();
			var request1 = new AuthRequest();
			request1.Parameters[SrpProtocolConstants.UserNameKey] = UserName;
			request1.Parameters[SrpProtocolConstants.ClientPublicEphemeralKey] = clientEphemeral.Public;

			// step1 response: Host -> User: s, B = kv + g^b (sends salt, b = random number)
			var response1 = await client.Call(request1);
			var salt = response1.GetSalt();
			var serverPublicEphemeral = response1.GetServerPublicEphemeral();
			if (string.IsNullOrWhiteSpace(salt) || string.IsNullOrWhiteSpace(serverPublicEphemeral))
			{
				throw new AuthFailedException("Server doesn't support SRP authentication protocol");
			}

			// step2 request: User -> Host: M = H(H(N) xor H(g), H(I), s, A, B, K)
			var privateKey = SrpClient.DerivePrivateKey(salt, UserName, Password);
			var clientSession = SrpClient.DeriveSession(clientEphemeral.Secret, serverPublicEphemeral, salt, UserName, privateKey);
			var request2 = new AuthRequest();
			request2.Parameters[SrpProtocolConstants.ClientSessionProofKey] = clientSession.Proof;

			// step2 response: Host -> User: H(A, M, K)
			var response2 = await client.Call(request2);
			var serverSessionProof = response2.GetServerSessionProof();
			SrpClient.VerifySession(clientEphemeral.Public, clientSession, serverSessionProof);
		}
	}
}
