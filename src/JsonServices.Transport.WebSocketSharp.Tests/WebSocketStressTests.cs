using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Exceptions;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Transport.WebSocketSharp.Tests
{
	[TestFixture, Explicit]
	public class WebSocketStressTests : StressTests
	{
		const string Url = "ws://localhost:8795";

		protected override int MaxClientsWithExceptions => 30;

		protected override int MaxClientsWithoutExceptions => 30;

		protected override JsonServer CreateServer()
		{
			// websocket transport
			var server = new WebSocketServer(Url);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			var translator = new StubExceptionTranslator();
			return new JsonServer(server, provider, serializer, executor,
				exceptionTranslator: translator);
		}

		protected override JsonClient CreateClient(JsonServer server)
		{
			var client = new WebSocketClient(Url);
			var serializer = new Serializer();
			var provider = new StubMessageTypeProvider();
			return new JsonClient(client, provider, serializer);
		}
	}
}
