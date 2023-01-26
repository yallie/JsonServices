﻿using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Exceptions;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Auth.SecureRemotePassword.Tests
{
	[TestFixture, Explicit]
	public class SrpStressTests : StressTests
	{
		private SrpAuthProvider AuthProvider { get; } =
			new SrpAuthProvider(new StubAccountRepository("demo", "12345"));

		private SrpCredentials Credentials =>
			new SrpCredentials("demo", "12345");

		protected override int MaxClientsWithExceptions => 200;

		protected override int MaxClientsWithoutExceptions => 200;

		protected override JsonServer CreateServer()
		{
			// fake transport
			var server = new StubServer();
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			var translator = new StubExceptionTranslator();
			return new JsonServer(server, provider, serializer, executor,
				AuthProvider, exceptionTranslator: translator);
		}

		protected override JsonClient CreateClient(JsonServer server)
		{
			var client = new StubClient(server.Server as StubServer);
			var serializer = new Serializer();
			var provider = new StubMessageTypeProvider();
			return new JsonClient(client, provider, serializer);
		}

		protected override void MultipleClientsSimpleCalls(int maxClients, bool allowExceptions, ICredentials credentials = null)
		{
			base.MultipleClientsSimpleCalls(maxClients, allowExceptions, credentials ?? Credentials);
		}
	}
}
