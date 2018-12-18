using System.Threading.Tasks;
using JsonServices.Tests;
using JsonServices.Tests.Messages.Generic;
using JsonServices.Tests.Services;
using NUnit.Framework;
using Serializer = JsonServices.Serialization.ServiceStack.Serializer;
using WebSocketClient = JsonServices.Transport.WebSocketSharp.WebSocketClient;

namespace JsonServices.Transport.Fleck.Tests
{
	[TestFixture]
	public class FleckServerTests : JsonServerTests
	{
		[Test]
		public async Task CallGetVersionServiceUsingFleckServer()
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8788");
			var client = new WebSocketClient("ws://127.0.0.1:8788");
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
		public async Task CallCalculateServiceUsingFleckServer()
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8789");
			var client = new WebSocketClient("ws://127.0.0.1:8789");
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
		public async Task CallUnregisteredServiceUsingFleckServer()
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8789");
			var client = new WebSocketClient("ws://127.0.0.1:8789");
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
		public async Task JsonServerCanExecuteGenericMessagesUsingFleckServer()
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8789");
			var client = new WebSocketClient("ws://127.0.0.1:8789");
			var serializer = new Serializer();
			var executor = new GenericServiceExecutor();
			var provider = new GenericMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallGenericMessagesCore(js, jc);
			}
		}
	}
}
