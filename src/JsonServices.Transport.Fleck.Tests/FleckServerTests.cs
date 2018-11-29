using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using NUnit.Framework;
using Serializer = JsonServices.Serialization.ServiceStack.Serializer;
using WebSocketClient = JsonServices.Transport.WebSocketSharp.WebSocketClient;

namespace JsonServices.Transport.Fleck.Tests
{
	[TestFixture]
	public class FleckServerTests
	{
		[Test]
		public async Task CallGetVersionService()
		{
			// websocket transport
			var server = new FleckServer("ws://127.0.0.1:8788");
			var client = new WebSocketClient("ws://127.0.0.1:8788");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor).Start())
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await jc.ConnectAsync();

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
			var server = new FleckServer("ws://127.0.0.1:8789");
			var client = new WebSocketClient("ws://127.0.0.1:8789");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor).Start())
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await jc.ConnectAsync();

				// normal call
				var msg = new Calculate
				{
					FirstOperand = 353,
					SecondOperand = 181,
					Operation = "+",
				};

				var result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(534, result.Result);

				msg.SecondOperand = 333;
				result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(686, result.Result);

				msg.Operation = "-";
				result = await jc.Call(msg);
				Assert.NotNull(result);
				Assert.AreEqual(20, result.Result);

				// call with error
				msg.Operation = "#";
				var ex = Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));

				// internal server error
				Assert.AreEqual(-32603, ex.Code);
				Assert.AreEqual("Internal server error", ex.Message);

				// call with another error
				msg.Operation = "%";
				msg.SecondOperand = 0;
				ex = Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));

				// internal server error
				Assert.AreEqual(-32603, ex.Code);
				Assert.AreEqual("Internal server error", ex.Message);

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

		// awaitable version of Assert.ThrowsAsync
		private async Task<T> Assert_ThrowsAsync<T>(AsyncTestDelegate code)
			where T : Exception
		{
			try
			{
				await code();
				Assert.Fail("Exception is expected but not thrown.");
				throw new InvalidOperationException();
			}
			catch (T ex)
			{
				// great, everything is fine
				return ex;
			}
			catch (Exception ex)
			{
				Assert.Fail($"Exception of type {typeof(T).Name} is expected, but {ex.GetType().Name} is thrown instead.");
				throw;
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
	}
}
