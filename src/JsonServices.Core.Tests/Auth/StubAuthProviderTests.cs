using System.Threading.Tasks;
using JsonServices.Auth;
using JsonServices.Exceptions;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Services;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Tests.Auth
{
	[TestFixture]
	public class StubAuthProviderTests
	{
		[Test]
		public void UserNameAndPasswordAreNotRequiredForAnonymous()
		{
			Assert.DoesNotThrow(() => new CredentialsBase());
		}

		[Test]
		public void UserNameAndPasswordCanBeSpecified()
		{
			var c = new CredentialsBase
			{
				UserName = "root",
				Password = "s3cr3t",
			};

			Assert.AreEqual("root", c.UserName);
			Assert.AreEqual("s3cr3t", c.Password);
		}



		[Test]
		public async Task AuthenticationProviderIsCalledOnConnectAsync()
		{
			// fake transport and serializer
			var server = new StubServer();
			var serverSerializer = new Serializer();
			var serverProvider = new StubMessageTypeProvider();
			var executor = new StubExecutor();
			var authProvider = new StubAuthProvider("root", "s3cr3t");

			var client = new StubClient(server, "jc");
			var clientProvider = new StubMessageTypeProvider();
			var clientSerializer = new Serializer();

			// json server and client
			var js = new JsonServer(server, serverProvider, serverSerializer, executor, authProvider).Start();
			var jc = new JsonClient(client, clientProvider, clientSerializer);
			Assert.IsFalse(authProvider.IsCalled);

			// connect
			await jc.ConnectAsync(new CredentialsBase("root", "s3cr3t"));
			Assert.IsTrue(authProvider.IsCalled);
		}
	}
}
