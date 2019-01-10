using System;
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
		public Task CallGetVersionServiceUsingFleckServerAndWebSocketSharpClient() =>
			CallGetVersionServiceCore(url => new WebSocketClient(url));

		[Test]
		public Task CallGetVersionServiceUsingFleckServerAndFleckClient() =>
			CallGetVersionServiceCore(url => new FleckClient(url));

		private async Task CallGetVersionServiceCore(Func<string, IClient> clientFactory)
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8788");
			var client = clientFactory("ws://127.0.0.1:8788");
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
		public Task CallCalculateServiceUsingFleckServerAndWebSocketSharpClient() =>
			CallCalculateServiceCore(url => new WebSocketClient(url));

		[Test]
		public Task CallCalculateServiceUsingFleckServerAndFleckClient() =>
			CallCalculateServiceCore(url => new FleckClient(url));

		private async Task CallCalculateServiceCore(Func<string, IClient> clientFactory)
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8789");
			var client = clientFactory("ws://127.0.0.1:8789");
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
		public Task CallUnregisteredServiceUsingFleckServerAndWebSocketSharpClient() =>
			CallUnregisteredServiceCore(url => new WebSocketClient(url));

		[Test]
		public Task CallUnregisteredServiceUsingFleckServerAndFleckClient() =>
			CallUnregisteredServiceCore(url => new FleckClient(url));

		private async Task CallUnregisteredServiceCore(Func<string, IClient> clientFactory)
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8789");
			var client = clientFactory("ws://127.0.0.1:8789");
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
		public Task JsonServerCanExecuteGenericMessagesUsingFleckServerAndWebSocketSharpClient() =>
			CallGenericMessagesCore(url => new WebSocketClient(url));

		[Test]
		public Task JsonServerCanExecuteGenericMessagesUsingFleckServerAndFleckClient() =>
			CallGenericMessagesCore(url => new FleckClient(url));

		private async Task CallGenericMessagesCore(Func<string, IClient> clientFactory)
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8789");
			var client = clientFactory("ws://127.0.0.1:8789");
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
