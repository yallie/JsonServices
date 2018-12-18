using System;
using System.Threading.Tasks;
using JsonServices.Tests;
using JsonServices.Tests.Messages.Generic;
using JsonServices.Tests.Services;
using NetMQ;
using NUnit.Framework;
using Serializer = JsonServices.Serialization.ServiceStack.Serializer;

namespace JsonServices.Transport.NetMQ.Tests
{
	[TestFixture]
	public class NetMQServerTests : JsonServerTests
	{
		[Test]
		public async Task CallGetVersionServiceUsingNetMQServer()
		{
			// NetMQ transport
			var server = new NetMQServer("tcp://127.0.0.1:8791");
			var client = new NetMQClient("tcp://127.0.0.1:8791");
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
		public async Task CallCalculateServiceUsingNetMQServer()
		{
			// NetMQ transport
			var server = new NetMQServer("tcp://127.0.0.1:8792");
			var client = new NetMQClient("tcp://127.0.0.1:8792");
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
		public async Task CallUnregisteredServiceUsingNetMQServer()
		{
			// NetMQ transport
			var server = new NetMQServer("tcp://127.0.0.1:8792");
			var client = new NetMQClient("tcp://127.0.0.1:8792");
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
		public async Task JsonServerAwaitsTasksUsingNetMQServer()
		{
			// NetMQ transport
			var server = new NetMQServer("tcp://127.0.0.1:8792");
			var client = new NetMQClient("tcp://127.0.0.1:8792");
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

		[Test]
		public async Task JsonServerCanExecuteGenericMessagesUsingNetMQServer()
		{
			// NetMQ transport
			var server = new NetMQServer("tcp://127.0.0.1:8792");
			var client = new NetMQClient("tcp://127.0.0.1:8792");
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

		public override void Dispose()
		{
			try
			{
				// fix unit test AppDomain unloading issue
				NetMQConfig.Cleanup(false);
			}
			catch (ObjectDisposedException)
			{
			}
		}
	}
}
