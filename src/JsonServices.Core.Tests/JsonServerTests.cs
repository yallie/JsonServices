using System;
using System.Threading.Tasks;
using JsonServices.Auth;
using JsonServices.Exceptions;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;
using Serializer = JsonServices.Serialization.ServiceStack.Serializer;

namespace JsonServices.Tests
{
	[TestFixture]
	public class JsonServerTests : TestFixtureBase
	{
		[Test]
		public void JsonServerRequiresServices()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonServer(null, null, null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), null, null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), new StubMessageTypeProvider(), null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), new StubMessageTypeProvider(), new Serializer(), null));
		}

		[Test]
		public async Task CallGetVersionService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			var js = new JsonServer(server, provider, serializer, executor);
			var jc = new JsonClient(client, provider, serializer);
			await CallGetVersionServiceCore(js, jc);
		}

		protected async Task CallGetVersionServiceCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			// event handlers
			var connected = 0;
			var disconnected = 0;
			js.ClientConnected += (s, e) => connected++;
			js.ClientDisconnected += (s, e) => disconnected++;
			js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");
			jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception: {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");

			// start json server and connect the client
			js.Start();
			await jc.ConnectAsync(credentials);

			// call GetVersion
			var msg = new GetVersion();
			var result = await Assert_NotTimedOut(jc.Call(msg), "jc.Call(msg)");
			Assert.NotNull(result);
			Assert.AreEqual("0.01-alpha", result.Version);

			// call GetVersion
			msg = new GetVersion { IsInternal = true };
			result = await Assert_NotTimedOut(jc.Call(msg), "jc.Call(msg...IsInternal)");
			Assert.NotNull(result);
			Assert.AreEqual("Version 0.01-alpha, build 12345, by yallie", result.Version);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
		}

		[Test]
		public async Task CallCalculateService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			var js = new JsonServer(server, provider, serializer, executor);
			var jc = new JsonClient(client, provider, serializer);
			await CallCalculateServiceCore(js, jc);
		}

		protected async Task CallCalculateServiceCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			// unhandled exception handlers
			var connected = 0;
			var disconnected = 0;
			js.ClientConnected += (s, e) => connected++;
			js.ClientDisconnected += (s, e) => disconnected++;
			js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");
			jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception: {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");

			// start json server and connect the client
			js.Start();
			await jc.ConnectAsync(credentials);

			// normal call
			var msg = new Calculate
			{
				FirstOperand = 353,
				SecondOperand = 181,
				Operation = "+",
			};

			var result = await Assert_NotTimedOut(jc.Call(msg), "353 + 181");
			Assert.NotNull(result);
			Assert.AreEqual(534, result.Result);

			msg.SecondOperand = 333;
			result = await Assert_NotTimedOut(jc.Call(msg), "353 + 333");
			Assert.NotNull(result);
			Assert.AreEqual(686, result.Result);

			msg.Operation = "-";
			result = await Assert_NotTimedOut(jc.Call(msg), "353 - 333");
			Assert.NotNull(result);
			Assert.AreEqual(20, result.Result);

			// call with error
			msg.Operation = "#";
			var ex = Assert.ThrowsAsync<InternalErrorException>(async () =>
				await Assert_NotTimedOut(jc.Call(msg), "353 # 333"));

			// internal server error
			Assert.AreEqual(-32603, ex.Code);
			Assert.AreEqual("Internal server error", ex.Message);

			// call with another error
			msg.Operation = "%";
			msg.SecondOperand = 0;
			ex = Assert.ThrowsAsync<InternalErrorException>(async () =>
				await Assert_NotTimedOut(jc.Call(msg), "353 % 0"));

			// internal server error
			Assert.AreEqual(-32603, ex.Code);
			Assert.AreEqual("Internal server error", ex.Message);

			// normal call again
			msg.Operation = "*";
			result = await Assert_NotTimedOut(jc.Call(msg), "353 * 0");
			Assert.NotNull(result);
			Assert.AreEqual(0, result.Result);

			msg.Operation = "+";
			msg.SecondOperand = 181;
			result = await Assert_NotTimedOut(jc.Call(msg), "353 + 181 again");
			Assert.NotNull(result);
			Assert.AreEqual(534, result.Result);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
		}

		[Test]
		public async Task CallUnregisteredService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			var js = new JsonServer(server, provider, serializer, executor);
			var jc = new JsonClient(client, provider, serializer);
			await CallUnregisteredServiceCore(js, jc);
		}

		public class UnregisteredService : IReturnVoid
		{
		}

		protected async Task CallUnregisteredServiceCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			// event handlers
			var connected = 0;
			var disconnected = 0;
			js.ClientConnected += (s, e) => connected++;
			js.ClientDisconnected += (s, e) => disconnected++;
			js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");
			jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception: {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");

			// start json server and connect the client
			js.Start();
			await jc.ConnectAsync(credentials);

			// call UnregisteredService
			var msg = new UnregisteredService();
			var ex = Assert.ThrowsAsync<MethodNotFoundException>(async () =>
				await Assert_NotTimedOut(jc.Call(msg), "jc.Call(UnregisteredService)"));
			Assert.AreEqual(MethodNotFoundException.ErrorCode, ex.Code);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
		}
	}
}
