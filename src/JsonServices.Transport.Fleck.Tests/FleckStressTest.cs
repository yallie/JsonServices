using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Transport.Fleck.Tests
{
	[TestFixture, Explicit]
	public class FleckStressTests : StressTests
	{
		const string Url = "ws://127.0.0.1:8794";

		protected override int MaxClientsWithExceptions => 100;

		protected override int MaxClientsWithoutExceptions => 500;

		protected override JsonServer CreateServer()
		{
			// websocket transport
			var server = new FleckServer(Url);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			return new JsonServer(server, provider, serializer, executor);
		}

		protected override JsonClient CreateClient(JsonServer server)
		{
			var client = new FleckClient(Url);
			var serializer = new Serializer();
			var provider = new StubMessageTypeProvider();
			return new JsonClient(client, provider, serializer);
		}
	}
}
