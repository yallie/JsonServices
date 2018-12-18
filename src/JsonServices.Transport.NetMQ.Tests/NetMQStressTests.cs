using System;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Services;
using NetMQ;
using NUnit.Framework;

namespace JsonServices.Transport.NetMQ.Tests
{
	[TestFixture, Explicit]
	public class NetMQStressTests : StressTests
	{
		const string Url = "tcp://127.0.0.1:8796";

		protected override int MaxClientsWithExceptions => 30;

		protected override int MaxClientsWithoutExceptions => 30;

		protected override JsonServer CreateServer()
		{
			// NetMQ transport
			var server = new NetMQServer(Url);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			return new JsonServer(server, provider, serializer, executor);
		}

		protected override JsonClient CreateClient(JsonServer server)
		{
			var client = new NetMQClient(Url);
			var serializer = new Serializer();
			var provider = new StubMessageTypeProvider();
			return new JsonClient(client, provider, serializer);
		}

		[TearDown]
		public void Teardown()
		{
			try
			{
				// fix unit test AppDomain unloading issue
				NetMQConfig.Cleanup();
			}
			catch (ObjectDisposedException)
			{
			}
		}
	}
}
