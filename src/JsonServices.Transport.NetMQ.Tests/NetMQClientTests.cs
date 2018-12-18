using System;
using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Services;
using NetMQ;
using NUnit.Framework;

namespace JsonServices.Transport.NetMQ.Tests
{
	[TestFixture]
	public class NetMQClientTests : JsonClientTests
	{
		[Test]
		public async Task JsonClientSupportsSubscriptionsAndUnsubscriptionsUsingNetMQServer()
		{
			// ZeroMQ tcp transport
			var server = new NetMQServer("tcp://127.0.0.1:8793");
			var client = new NetMQClient("tcp://127.0.0.1:8793");
			var secondClient = new NetMQClient("tcp://127.0.0.1:8793");
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
			// ZeroMQ tcp transport
			var server = new NetMQServer("tcp://127.0.0.1:8794");
			var client = new NetMQClient("tcp://127.0.0.1:8794");
			var secondClient = new NetMQClient("tcp://127.0.0.1:8794");
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

		[Test]
		public async Task JsonClientCanDisconnectAndReconnectUsingUsingNetMQServer()
		{
			// ZeroMQ tcp transport
			var server = new NetMQServer("tcp://127.0.0.1:8794");
			var client = new NetMQClient("tcp://127.0.0.1:8794");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDisconnectAndReconnectCore(js, jc);
			}
		}

		[Test, Explicit("Fails on CI server?")]
		public async Task JsonClientRejectsPendingMessagesWhenDisconnectedUsingNetMQServer()
		{
			// ZeroMQ tcp transport
			var server = new NetMQServer("tcp://127.0.0.1:8794");
			var client = new NetMQClient("tcp://127.0.0.1:8794");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDelayServiceAndDisconnectCore(js, jc);
			}
		}

		[Test, Explicit("Fails on CI server?")]
		public async Task JsonClientRejectsPendingMessagesWhenConnectionIsAbortedUsingNetMQServer()
		{
			// ZeroMQ tcp transport
			var server = new NetMQServer("tcp://127.0.0.1:8794");
			var client = new NetMQClient("tcp://127.0.0.1:8794");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDelayServiceAndAbortConnectionCore(js, jc);
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
