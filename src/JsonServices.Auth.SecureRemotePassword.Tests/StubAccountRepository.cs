using System.Security.Principal;
using SecureRemotePassword;

namespace JsonServices.Auth.SecureRemotePassword.Tests
{
	public class StubAccountRepository : ISrpAccountRepository
	{
		public StubAccountRepository(string userName, string password, SrpParameters parameters = null)
		{
			// create sample user account
			var srpClient = new SrpClient(parameters);
			var salt = srpClient.GenerateSalt();
			var privateKey = srpClient.DerivePrivateKey(salt, userName, password);
			var verifier = srpClient.DeriveVerifier(privateKey);
			SampleAccount = new SrpAccount
			{
				UserName = userName,
				Salt = salt,
				Verifier = verifier,
			};
		}

		private SrpAccount SampleAccount { get; set; }

		public ISrpAccount FindByName(string userName)
		{
			if (SampleAccount.UserName == userName)
			{
				return SampleAccount;
			}

			return null;
		}

		public IIdentity GetIdentity(ISrpAccount account)
		{
			return new GenericIdentity(account.UserName);
		}

		private class SrpAccount : ISrpAccount
		{
			public string UserName { get; set; }
			public string Salt { get; set; }
			public string Verifier { get; set; }
		}
	}
}
