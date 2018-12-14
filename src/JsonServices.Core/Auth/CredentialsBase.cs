using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonServices.Auth
{
	public class CredentialsBase : ICredentials
	{
		public CredentialsBase()
		{
		}

		public CredentialsBase(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		public string UserName
		{
			get { return Parameters[AuthRequest.UserNameKey]; }
			set { Parameters[AuthRequest.UserNameKey] = value; }
		}

		public string Password
		{
			get { return Parameters[AuthRequest.PasswordKey]; }
			set { Parameters[AuthRequest.PasswordKey] = value; }
		}

		public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();

		public virtual async Task<string> Authenticate(JsonClient client)
		{
			var response = await client.Call(new AuthRequest
			{
				Parameters = Parameters,
			});

			return response.SessionId;
		}
	}
}
