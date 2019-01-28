using System;
using System.Threading.Tasks;
using JsonServices.Auth;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Messages.Generic;
using JsonServices.Tests.Serialization;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using JsonServices.Transport;
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
		public async Task JsonServerHandlesDeserializationErrors()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var provider = new StubMessageTypeProvider();
			var serverSerializer = new BrokenSerializer();
			var clientSerializer = new Serializer();
			var executor = new StubExecutor();

			using (var js = new JsonServer(server, provider, serverSerializer, executor))
			using (var jc = new JsonClient(client, provider, clientSerializer))
			{
				js.Start();

				var tcs = new TaskCompletionSource<bool>();
				js.UnhandledException += (s, e) => tcs.TrySetException(e.Exception);

				// TODO: can we have something better than a timeout here?
				await Assert_TimedOut(jc.ConnectAsync(), timeout: Task.Delay(200));

				// the server should have got an unhandled exception
				Assert.ThrowsAsync<NotImplementedException>(async () => await Assert_NotTimedOut(tcs.Task));
			}
		}

		[Test]
		public void JsonServerHandlesMessageTypeProvidersErrors()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var provider = new BrokenMessageTypeProvider();
			var serverSerializer = new Serializer();
			var clientSerializer = new Serializer();
			var executor = new StubExecutor();

			using (var js = new JsonServer(server, provider, serverSerializer, executor))
			using (var jc = new JsonClient(client, provider, clientSerializer))
			{
				js.Start();

				var ex = Assert.ThrowsAsync<InvalidRequestException>(() => Assert_NotTimedOut(jc.ConnectAsync(), timeout: Task.Delay(200)));

				// note: JsonServicesException.MessageId is lost
				// when an exception is translated to Error and back again
				Assert.IsNull(ex.MessageId);
			}
		}

		[Test]
		public async Task JsonServerHasEvents()
		{
			var server = new StubServer();
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();
			var js = new JsonServer(server, provider, serializer, executor);

			var connectCounter = 0;
			void connectEventHandler(object sender, MessageEventArgs e) => connectCounter++;
			js.ClientConnected += connectEventHandler;
			js.ClientDisconnected += connectEventHandler;

			var serviceCounter = 0;
			void serviceEventHandler(object sender, EventArgs e) => serviceCounter++;
			js.InitializeRequestContext += serviceEventHandler;
			js.BeforeExecuteService += serviceEventHandler;
			js.AfterExecuteService += serviceEventHandler;

			var client = new StubClient(server);
			var jc = new JsonClient(client, provider, serializer);
			Assert.AreEqual(1, connectCounter);
			Assert.AreEqual(0, serviceCounter);

			// connect makes one service call and fires three events:
			// InitializeRequestContext, BeforeExecuteService, AfterExecuteService
			await jc.ConnectAsync();
			Assert.AreEqual(3, serviceCounter);

			js.Dispose();
			Assert.AreEqual(2, connectCounter);

			// unsubscribe
			js.ClientDisconnected -= connectEventHandler;
			js.ClientConnected -= connectEventHandler;
			js.AfterExecuteService -= serviceEventHandler;
			js.BeforeExecuteService -= serviceEventHandler;
			js.InitializeRequestContext -= serviceEventHandler;
		}

		[Test]
		public async Task CallServiceBeforeConnectingShouldFail()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			var js = new JsonServer(server, provider, serializer, executor);
			var jc = new JsonClient(client, provider, serializer);
			js.Start();

			Assert.ThrowsAsync<AuthRequiredException>(async () =>
				await Assert_NotTimedOut(jc.Call(new GetVersion()), "jc.Call(GetVersion) before Connect"));

			await Assert_NotTimedOut(jc.ConnectAsync(), "jc.ConnectAsync()");
			await Assert_NotTimedOut(jc.Call(new GetVersion()), "jc.Call(GetVersion) after connect");
		}

		[Test]
		public async Task CallBuiltinVersionService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallBuiltinVersionServiceCore(js, jc);
			}
		}

		private async Task CallBuiltinVersionServiceCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
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

			Assert.IsNull(jc.SessionId);
			var sessionId = await jc.ConnectAsync(credentials);
			Assert.IsNotNull(jc.SessionId);
			Assert.AreEqual(sessionId, jc.SessionId);

			// call Version
			var msg = new VersionRequest();
			var result = await Assert_NotTimedOut(jc.Call(msg), "jc.Call(VersionRequest)");
			Assert.NotNull(result);
			Assert.AreEqual(nameof(JsonServices), result.ProductName);
			Assert.NotNull(result.ProductVersion);
			Assert.NotNull(result.EngineVersion);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
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

			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallGetVersionServiceCore(js, jc);
			}
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

			Assert.IsNull(jc.SessionId);
			var sessionId = await jc.ConnectAsync(credentials);
			Assert.IsNotNull(jc.SessionId);
			Assert.AreEqual(sessionId, jc.SessionId);

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
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallCalculateServiceCore(js, jc);
			}
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
			Assert.AreEqual("Internal server error: Bad operation: #", ex.Message);

			// call with another error
			msg.Operation = "%";
			msg.SecondOperand = 0;
			ex = Assert.ThrowsAsync<InternalErrorException>(async () =>
				await Assert_NotTimedOut(jc.Call(msg), "353 % 0"));

			// internal server error
			Assert.AreEqual(-32603, ex.Code);
			Assert.AreEqual("Internal server error: Attempted to divide by zero.", ex.Message);

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
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallUnregisteredServiceCore(js, jc);
			}
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

		[Test]
		public async Task JsonServerAwaitsTasks()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDelayServiceCore(js, jc);
			}
		}

		protected virtual async Task CallDelayServiceCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			js.Start();
			await jc.ConnectAsync(credentials);
			await Assert_NotTimedOut(jc.Call(new DelayRequest { Milliseconds = 10 }), "jc.Call(Delay 10)");
			await Assert_TimedOut(jc.Call(new DelayRequest { Milliseconds = 200 }), "jc.Call(Delay 200)", Task.Delay(10));

			// make sure that await completes before the server is disposed (affects NetMQ server)
			await Task.Delay(300);
		}

		[Test]
		public async Task JsonServerCanExecuteGenericMessages()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new GenericServiceExecutor();
			var provider = new GenericMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallGenericMessagesCore(js, jc);
			}
		}

		protected async Task CallGenericMessagesCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			js.Start();
			await jc.ConnectAsync();

			var intMsg = new GenericRequest<int> { Value = 1 };
			var intResult = await jc.Call(intMsg);
			Assert.AreEqual(2, intResult.Result);

			var dtMsg = new GenericRequest<DateTime> { Value = new DateTime(2018, 12, 18) };
			var dtResult = await jc.Call(dtMsg);
			Assert.AreEqual(new DateTime(2019, 12, 18), dtResult.Result);

			var strMsg = new GenericRequest<string> { Value = "World" };
			var strResult = await jc.Call(strMsg);
			Assert.AreEqual("Hello World!", strResult.Result);

			var boolMsg = new GenericRequest<bool> { Value = true };
			Assert.ThrowsAsync<MethodNotFoundException>(async () => await jc.Call(boolMsg));

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
		}
	}
}
