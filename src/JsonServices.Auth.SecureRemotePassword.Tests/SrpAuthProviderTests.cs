using JsonServices.Exceptions;
using JsonServices.Services;
using NUnit.Framework;

namespace JsonServices.Auth.SecureRemotePassword.Tests
{
	[TestFixture]
	public class SrpAuthProviderTests
	{
		private SrpAuthProvider AuthProvider { get; } =
			new SrpAuthProvider(new StubAccountRepository("hacker", "pa55w0rd"));

		[Test]
		public void SrpAuthProviderThrowsOnInvalidRequestMessages()
		{
			// reset RequestContext to make sure the current ConnectionId is not set
			RequestContext.CurrentContextHolder.Value = null;

			// null is not allowed
			Assert.Throws<AuthFailedException>(() => AuthProvider.Authenticate(null));

			// empty parameters
			var authRequest = new AuthRequest();
			Assert.Throws<AuthFailedException>(() => AuthProvider.Authenticate(authRequest));

			// login session not specified
			authRequest.Parameters[SrpProtocolConstants.UserNameKey] = "Bozo";
			Assert.Throws<AuthFailedException>(() => AuthProvider.Authenticate(authRequest));

			// protocol error: client public ephemeral or client session proof is expected
			authRequest.SetLoginSession("321");
			Assert.Throws<AuthFailedException>(() => AuthProvider.Authenticate(authRequest));
		}

		[Test]
		public void UnknownUserDoesntFailTheFirstAuthStep()
		{
			var authRequest = new AuthRequest();
			authRequest.SetLoginSession("123");
			authRequest.Parameters[SrpProtocolConstants.UserNameKey] = "root";
			authRequest.Parameters[SrpProtocolConstants.ClientPublicEphemeralKey] = "123";

			// server generates fake salt and ephemeral values
			var authResponse = AuthProvider.Authenticate(authRequest);
			Assert.IsTrue(AuthProvider.PendingAuthentications.IsEmpty);
			Assert.IsNotNull(authResponse);
			Assert.IsNotNull(authResponse.GetSalt());
			Assert.IsNotNull(authResponse.GetServerPublicEphemeral());

			var firstSalt = authResponse.GetSalt();
			var firstEphemeral = authResponse.GetServerPublicEphemeral();

			// retry the first step for the same user
			authRequest.SetLoginSession("321");
			authResponse = AuthProvider.Authenticate(authRequest);
			Assert.IsTrue(AuthProvider.PendingAuthentications.IsEmpty);
			Assert.IsNotNull(authResponse);
			Assert.IsNotNull(authResponse.GetSalt());
			Assert.IsNotNull(authResponse.GetServerPublicEphemeral());

			// same fake salt, but another fake ephemeral is expected
			Assert.AreEqual(firstSalt, authResponse.GetSalt());
			Assert.AreNotEqual(firstEphemeral, authResponse.GetServerPublicEphemeral());
		}
	}
}
