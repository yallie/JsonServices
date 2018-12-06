using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Transport.WebSocketSharp.Tests
{
	[TestFixture]
	public class WebSocketClientTests : JsonClientTests
	{
		[Test]
		public async Task JsonClientSupportsSubscriptionsAndUnsubscriptionsUsingWebSocketSharp()
		{
			// websocket-sharp transport
			var server = new WebSocketServer("ws://localhost:8768");
			var client = new WebSocketClient("ws://localhost:8768");
			var secondClient = new WebSocketClient("ws://localhost:8768");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			using (var sc = new JsonClient(secondClient, provider, serializer))
			{
				// set names for easier debugging
				jc.DebugName = "First";
				sc.DebugName = "Second";

				// execute core test
				await TestSubscriptionsAndUnsubscriptionsCore(js, jc, sc);
			}
		}

		[Test]
		public async Task JsonClientSupportsFilteredSubscriptionsAndUnsubscriptionsUsingNetMQServer()
		{
			// websocket-sharp transport
			var server = new WebSocketServer("ws://localhost:8768");
			var client = new WebSocketClient("ws://localhost:8768");
			var secondClient = new WebSocketClient("ws://localhost:8768");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			using (var sc = new JsonClient(secondClient, provider, serializer))
			{
				// set names for easier debugging
				jc.DebugName = "First";
				sc.DebugName = "Second";

				// execute core test
				await TestFilteredSubscriptionsAndUnsubscriptionsCore(js, jc, sc);
			}
		}
	}
}
