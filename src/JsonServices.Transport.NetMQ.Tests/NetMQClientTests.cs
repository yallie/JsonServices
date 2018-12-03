using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Transport.NetMQ.Tests
{
	[TestFixture]
	public class NetMQClientTests : JsonClientTests
	{
		[Test]
		public async Task JsonClientSupportsSubscriptionsAndUnsubscriptionsUsingNetMQServer()
		{
			// websocket transport
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
	}
}
