using System.Threading.Tasks;
using JsonServices.Tests;
using JsonServices.Tests.Services;
using NUnit.Framework;
using Serializer = JsonServices.Serialization.ServiceStack.Serializer;

namespace JsonServices.Transport.WebSocketSharp.Tests
{
	[TestFixture]
	public class WebSocketServerTests : JsonServerTests
	{
		[Test]
		public async Task CallGetVersionServiceUsingWebSocketSharp()
		{
			// websocket-sharp transport
			var server = new WebSocketServer("ws://localhost:8766");
			var client = new WebSocketClient("ws://localhost:8766");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallGetVersionServiceCore(js, jc);
			}
		}

		[Test]
		public async Task CallCalculateServiceUsingWebSocketSharp()
		{
			// websocket-sharp transport
			var server = new WebSocketServer("ws://localhost:8767");
			var client = new WebSocketClient("ws://localhost:8767");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallCalculateServiceCore(js, jc);
			}
		}

		[Test]
		public async Task CallUnregisteredServiceUsingWebSocketSharp()
		{
			// websocket-sharp transport
			var server = new WebSocketServer("ws://localhost:8767");
			var client = new WebSocketClient("ws://localhost:8767");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallUnregisteredServiceCore(js, jc);
			}
		}

		[Test]
		public async Task JsonServerAwaitsTasksUsingWebSocketSharp()
		{
			// websocket-sharp transport
			var server = new WebSocketServer("ws://localhost:8767");
			var client = new WebSocketClient("ws://localhost:8767");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDelayServiceCore(js, jc);
			}
		}
	}
}
