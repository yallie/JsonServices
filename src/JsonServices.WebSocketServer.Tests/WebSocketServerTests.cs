using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Serialization;
using JsonServices.Tests.Services;
using NUnit.Framework;
using StSerializer = JsonServices.Tests.Serialization.ServiceStack.Text.Serializer;

namespace JsonServices.WebSocketServer.Tests
{
	[TestFixture]
	public class WebSocketServerTests
	{
		[Test]
		public async Task CallGetVersionService()
		{
			// websocket transport
			var server = new WebSocketServer("ws://localhost:8765");
			var client = new WebSocketClient("ws://localhost:8765");
			var locator = new StubLocator();
			var serializer = new StSerializer(locator);
			var executor = new StubExecutor();

			// json server and client
			using (var js = new JsonServer(server, serializer, executor).Start())
			using (var jc = new JsonClient(client, serializer).Connect())
			{
				// call GetVersion
				var msg = new GetVersion();
				var result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual("0.01-alpha", result.Version);

				// call GetVersion
				msg = new GetVersion { IsInternal = true };
				result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual("Version 0.01-alpha, build 12345, by yallie", result.Version);
			}
		}

		[Test]
		public async Task CallCalculateService()
		{
			// websocket transport
			var server = new WebSocketServer("ws://localhost:8765");
			var client = new WebSocketClient("ws://localhost:8765");
			var locator = new StubLocator();
			var serializer = new StSerializer(locator);
			var executor = new StubExecutor();

			// json server and client
			using (var js = new JsonServer(server, serializer, executor).Start())
			using (var jc = new JsonClient(client, serializer).Connect())
			{
				// normal call
				var msg = new Calculate
				{
					FirstOperand = 353,
					SecondOperand = 181,
					Operation = "+",
				};

				var result = default(CalculateResponse); /*
				result await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(534, result.Result);

				msg.SecondOperand = 333;
				result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(686, result.Result);

				msg.Operation = "-";
				result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(20, result.Result);*/

				//Assert.AreEqual(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
				//Assert.IsNull(SynchronizationContext.Current);
				result = await jc.Call(msg);
				//Assert.AreEqual(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
				//Assert.IsNull(SynchronizationContext.Current);

				// Assert.ThrowAsync freezes if jc.Call was awaited before
				msg.Operation = "#";
				Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));
				Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));
				Assert.IsNull(SynchronizationContext.Current);

				// call with error
				msg.Operation = "#";
				try
				{
					await jc.Call(msg);
					Assert.Fail("Exception was expected.");
				}
				catch (JsonServicesException ex)
				{
					// internal server error
					Assert.AreEqual(-32603, ex.Code);
				}

				// call with another error
				msg.Operation = "%";
				msg.SecondOperand = 0;
				try
				{
					await jc.Call(msg);
					Assert.Fail("Exception was expected.");
				}
				catch (JsonServicesException ex)
				{
					// internal server error
					Assert.AreEqual(-32603, ex.Code);
				}

				//// TODO: Assert.ThrowsAsync doesn't work for some reason!
				//// call with error
				//msg.Operation = "#";
				//Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));

				//// call with another error
				//msg.Operation = "%";
				//msg.SecondOperand = 0;
				//Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));

				// normal call again
				msg.Operation = "*";
				result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(0, result.Result);

				msg.Operation = "+";
				msg.SecondOperand = 181;
				result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(534, result.Result);
			}
		}

		private Task<bool> AsyncOperation(bool throwException)
		{
			var tcs = new TaskCompletionSource<bool>();
			ThreadPool.QueueUserWorkItem(x =>
			{
				if (throwException)
				{
					tcs.SetException(new InvalidOperationException());
				}
				else
				{
					tcs.SetResult(true);
				}
			});

			return tcs.Task;
		}

		[Test]
		public async Task MakeSureNUnitDoesntDeadlock()
		{
			// works fine
			var result = await AsyncOperation(throwException: false);
			Assert.IsTrue(result);

			// suspected a deadlock here, but actually it works fine
			// so the problem is not NUnit, but either my code or WebSocketSharp
			Assert.ThrowsAsync<InvalidOperationException>(
				async () => await AsyncOperation(throwException: true));
		}
	}
}
