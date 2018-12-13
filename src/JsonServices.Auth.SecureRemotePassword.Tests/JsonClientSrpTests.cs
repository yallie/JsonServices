using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Auth.SecureRemotePassword.Tests
{
	[TestFixture]
	public class JsonClientSrpTests : JsonClientTests
	{
		private SrpAuthProvider AuthProvider { get; } =
			new SrpAuthProvider(new StubAccountRepository("admin", "secret"));

		private SrpCredentials Credentials { get; } =
			new SrpCredentials("admin", "secret");

		[Test]
		public async Task JsonClientSupportsSubscriptionsAndUnsubscriptionsUsingFleckServer()
		{
			// fake transport and serializer
			var server = new StubServer();
			var serverSerializer = new Serializer();
			var serverProvider = new StubMessageTypeProvider();
			var executor = new StubExecutor();

			var client = new StubClient(server, "jc");
			var clientProvider = new StubMessageTypeProvider();
			var clientSerializer = new Serializer();

			var secondClient = new StubClient(server, "sc");
			var secondClientProvider = new StubMessageTypeProvider();
			var secondClientSerializer = new Serializer();

			// json server and client
			using (var js = new JsonServer(server, serverProvider, serverSerializer, executor, AuthProvider))
			using (var jc = new JsonClient(client, clientProvider, clientSerializer))
			using (var sc = new JsonClient(secondClient, secondClientProvider, secondClientSerializer))
			{
				// set names for easier debugging
				jc.DebugName = "First";
				sc.DebugName = "Second";

				// execute core test
				await TestSubscriptionsAndUnsubscriptionsCore(js, jc, sc, Credentials);
			}
		}

		[Test]
		public async Task JsonClientSupportsFilteredSubscriptionsAndUnsubscriptionsUsingFleckServer()
		{
			// fake transport and serializer
			var server = new StubServer();
			var serverSerializer = new Serializer();
			var serverProvider = new StubMessageTypeProvider();
			var executor = new StubExecutor();

			var client = new StubClient(server, "jc");
			var clientProvider = new StubMessageTypeProvider();
			var clientSerializer = new Serializer();

			var secondClient = new StubClient(server, "sc");
			var secondClientProvider = new StubMessageTypeProvider();
			var secondClientSerializer = new Serializer();

			// json server and client
			using (var js = new JsonServer(server, serverProvider, serverSerializer, executor, AuthProvider))
			using (var jc = new JsonClient(client, clientProvider, clientSerializer))
			using (var sc = new JsonClient(secondClient, secondClientProvider, secondClientSerializer))
			{
				// set names for easier debugging
				jc.DebugName = "First";
				sc.DebugName = "Second";

				// execute core test
				await TestFilteredSubscriptionsAndUnsubscriptionsCore(js, jc, sc, Credentials);
			}
		}
	}
}
