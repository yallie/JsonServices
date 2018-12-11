using System;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture, Explicit]
	public class StressTests : TestFixtureBase
	{
		protected override Task Timeout => Task.Delay(5000); // adjust for large number of clients

		protected virtual int MaxClientsWithoutExceptions => 1000;

		protected virtual int MaxClientsWithExceptions => 100; // exceptions slow things down considerably

		protected virtual JsonServer CreateServer()
		{
			// fake transport and serializer
			var server = new StubServer();
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			return new JsonServer(server, provider, serializer, executor);
		}

		protected virtual JsonClient CreateClient(JsonServer server)
		{
			// fake transport and serializer
			var client = new StubClient((StubServer)server.Server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			return new JsonClient(client, provider, serializer);
		}

		protected async Task ClientRoutine(JsonClient client, int seed, bool allowExceptions)
		{
			var random = new Random(seed);
			var iterations = random.Next(20) + 10;
			for (var i = 0; i < iterations; i++)
			{
				if (random.Next(100) > 50)
				{
					await ExecuteGetVersion(client, random.Next(100) > 50);
				}
				else
				{
					// operations "/" and "@" may throw exceptions
					var operation = random.Next(allowExceptions ? 5 : 3);
					await ExecuteCalculate(client, random.Next(1000), operation, random.Next(1000));
				}

				await Task.Delay(random.Next(100));
			}

			// make sure no pending messages are left
			Assert.AreEqual(0, client.PendingMessages.Count);
		}

		private async Task ExecuteGetVersion(JsonClient client, bool isInternal)
		{
			var result = await Assert_NotTimedOut(client.Call(new GetVersion { IsInternal = isInternal }), "StressTest.GetVersion");
			var expected = isInternal ? "Version 0.01-alpha, build 12345, by yallie" : "0.01-alpha";
			Assert.NotNull(result);
			Assert.AreEqual(expected, result.Version);
		}

		private async Task ExecuteCalculate(JsonClient client, int first, int operation, int second)
		{
			// add unsupported operation '@'
			var msg = new Calculate
			{
				FirstOperand = first,
				Operation = $"{"+-*/@"[operation]}",
				SecondOperand = second,
			};

			// emulate the result on the client side
			var expectedResult = default(decimal?);
			try
			{
				expectedResult = new Func<decimal, decimal, decimal>[]
				{
					(a, b) => a + b,
					(a, b) => a - b,
					(a, b) => a * b,
					(a, b) => a / b,
					(a, b) => throw new InvalidOperationException()
				}[operation](first, second);
			}
			catch
			{
				expectedResult = null;
			}

			if (expectedResult.HasValue)
			{
				var result = await Assert_NotTimedOut(client.Call(msg), $"{first} {msg.Operation} {second}");
				Assert.NotNull(result);
				Assert.AreEqual(expectedResult.Value, result.Result);
			}
			else
			{
				Assert.ThrowsAsync<InternalErrorException>(async () =>
					await Assert_NotTimedOut(client.Call(msg), $"{first} {msg.Operation} {second}"));
			}
		}

		[Test]
		public void MultipleClientsSimpleCallsWithoutExceptions()
		{
			MultipleClientsSimpleCalls(MaxClientsWithoutExceptions, allowExceptions: false);
		}

		[Test]
		public void MultipleClientsSimpleCallsWithExceptions()
		{
			MultipleClientsSimpleCalls(MaxClientsWithExceptions, allowExceptions: true);
		}

		protected virtual void MultipleClientsSimpleCalls(int maxClients, bool allowExceptions)
		{
			Assert.Multiple(async () =>
			{
				using (var js = CreateServer().Start())
				{
					js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}.");

					var clients = Enumerable.Range(100, maxClients).Select(async seed =>
					{
						var jc = CreateClient(js);
						jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception: {e.Exception}.");

						await jc.ConnectAsync();
						await ClientRoutine(jc, seed, allowExceptions);
					});

					await Task.WhenAll(clients);
				}
			});
		}
	}
}
