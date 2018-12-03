using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Tests;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
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
			// websocket transport
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
			// websocket transport
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
	}
}
