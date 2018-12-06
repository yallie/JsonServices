using JsonServices.Auth;
using JsonServices.Services;
using NUnit.Framework;

namespace JsonServices.Tests.Auth
{
	public class StubAuthProvider : IAuthProvider
	{
		public StubAuthProvider(string userName, string password)
		{
			UserName = userName;
			Password = password;
		}

		public string UserName { get; set; }

		public string Password { get; set; }

		public bool IsCalled { get; set; }

		public AuthResponse Authenticate(ServiceExecutionContext context, AuthRequest authRequest)
		{
			Assert.IsNotNull(authRequest);
			Assert.IsNotNull(authRequest.Parameters);
			Assert.AreEqual(UserName, authRequest.Parameters[AuthRequest.UserNameKey]);
			Assert.AreEqual(Password, authRequest.Parameters[AuthRequest.PasswordKey]);

			IsCalled = true;
			return new AuthResponse(new AnonymousIdentity());
		}
	}
}
