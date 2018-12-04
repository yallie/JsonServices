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
				await TestFilteredSubscriptionsAndUnsubscriptionsCore(js, jc, sc);
			}
		}

		public override void Dispose()
		{
			// fix unit test AppDomain unloading issue
			NetMQConfig.Cleanup();
		}
	}
}
