using System;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Auth;
using JsonServices.Exceptions;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests.Exceptions;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class StressTests : TestFixtureBase
	{
		protected override Task Timeout => Task.Delay(TimeSpan.FromMinutes(1)); // adjusted so it doesn't timeout on CI server

		protected virtual int MaxClientsWithoutExceptions => 1000;

		protected virtual int MaxClientsWithExceptions => 100; // exceptions slow things down considerably

		protected virtual JsonServer CreateServer()
		{
			// fake transport and serializer
			var server = new StubServer();
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			var translator = new StubExceptionTranslator();
			return new JsonServer(server, provider, serializer, executor,
				exceptionTranslator: translator);
		}

		protected virtual JsonClient CreateClient(JsonServer server)
		{
			// fake transport and serializer
			var client = new StubClient((StubServer)server.Server);
			var serializer = new Serializer();
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

		protected virtual void MultipleClientsSimpleCalls(int maxClients, bool allowExceptions, ICredentials credentials = null)
		{
			Assert.Multiple(async () =>
			{
				using (var js = CreateServer().Start())
				{
					js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}.");

					var clients = Enumerable.Range(100, maxClients).Select(async seed =>
					{
						// connect the client
						var jc = CreateClient(js);
						jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception: {e.Exception}.");

						await jc.ConnectAsync(credentials);
						await ClientRoutine(jc, seed, allowExceptions);
					});

					await Task.WhenAll(clients);
				}
			});
		}

		[Test]
		public void FailingAfterExecutionHandler()
		{
			FailingAfterExecutionHandler(10, allowExceptions: false);
		}

		protected virtual void FailingAfterExecutionHandler(int maxClients, bool allowExceptions, ICredentials credentials = null)
		{
			Assert.Multiple(() =>
			{
				using (var js = CreateServer().Start())
				{
					js.AfterExecuteService += (s, e) => throw new InvalidOperationException("This exception shouldn't crash the server.");
					js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}.");

					var clients = Enumerable.Range(100, maxClients).Select(async seed =>
					{
						// connect the client
						var jc = CreateClient(js);
						jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception: {e.Exception}.");

						await jc.ConnectAsync(credentials);
						await ClientRoutine(jc, seed, allowExceptions);
					});

					// the server didn't crash, the client didn't freeze
					// and internal error exception is reported
					Assert.That(async () => await Task.WhenAll(clients),
						Throws.TypeOf<InternalErrorException>()
							.With.Property("Code").EqualTo(-32603));
				}
			});
		}

		[Test]
		public void FailingExceptionTranslator()
		{
			FailingExceptionTranslator(10, allowExceptions: true);
		}

		protected virtual void FailingExceptionTranslator(int maxClients, bool allowExceptions, ICredentials credentials = null)
		{
			Assert.Multiple(() =>
			{
				using (var js = CreateServer().Start())
				{
					js.AfterExecuteService += (s, e) => throw new InvalidOperationException("This exception shouldn't crash the server.");
					js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}.");

					var tr = js.ExceptionTranslator as StubExceptionTranslator;
					Assert.That(tr, Is.Not.Null);
					tr.ErrorTranslated += (s, e) => throw new InvalidOperationException($"Totally unexpected!");

					var clients = Enumerable.Range(100, maxClients).Select(async seed =>
					{
						// connect the client
						var jc = CreateClient(js);
						jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception: {e.Exception}.");

						await jc.ConnectAsync(credentials);
						await ClientRoutine(jc, seed, allowExceptions);
					});

					// the server didn't crash, the client didn't freeze
					// and internal error exception is reported
					Assert.That(async () => await Task.WhenAll(clients),
						Throws.TypeOf<InternalErrorException>()
							.With.Property("Code").EqualTo(-32603));
				}
			});
		}
	}
}
