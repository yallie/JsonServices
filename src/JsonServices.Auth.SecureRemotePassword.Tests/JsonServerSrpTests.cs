using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Auth.SecureRemotePassword.Tests
{
	[TestFixture]
	public class JsonServerSrpTests : JsonServerTests
	{
		private SrpAuthProvider AuthProvider { get; } =
			new SrpAuthProvider(new StubAccountRepository("hacker", "pa55w0rd"));

		private SrpCredentials Credentials { get; } =
			new SrpCredentials("hacker", "pa55w0rd");

		[Test]
		public void SrpAuthenticationFailsOnBadCredentials()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor, AuthProvider))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				// default credentials
				var ex = Assert.ThrowsAsync<AuthFailedException>(async () =>
					await CallGetVersionServiceCore(js, jc));
				Assert.AreEqual("No credentials specified", ex.Message);

				// invalid credentials
				ex = Assert.ThrowsAsync<AuthFailedException>(async () =>
					await CallGetVersionServiceCore(js, jc, new SrpCredentials("root", "beer")));
				Assert.AreEqual("Authentication failed", ex.Message);

				// invalid credentials
				ex = Assert.ThrowsAsync<AuthFailedException>(async () =>
					await CallGetVersionServiceCore(js, jc, new SrpCredentials("hacker", "password")));
				Assert.AreEqual("Authentication failed", ex.Message);
			}
		}

		[Test]
		public async Task CallGetVersionServiceUsingSrpAuthentication()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor, AuthProvider))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallGetVersionServiceCore(js, jc, Credentials);
			}
		}
	}
}
