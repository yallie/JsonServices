using System;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class TestFixtureBaseTests : TestFixtureBase
	{
		[Test]
		public async Task AssertNotTimedOutNormalExecution()
		{
			await Assert_NotTimedOut(Task.Delay(10));
		}

		[Test]
		public async Task AssertNotTimedOutErrorExecution()
		{
			await Assert_ThrowsAsync<CustomAssertionException>(async () =>
				await Assert_NotTimedOut(Task.Delay(10), "task", Task.FromResult(true)));
		}

		[Test]
		public async Task AssertNotTimedOutException()
		{
			await Assert_ThrowsAsync<Exception>(() =>
				Assert_NotTimedOut(Throw(new Exception("Error"))));
		}

		private async Task Throw(Exception ex, Task delay = null)
		{
			if (delay != null)
			{
				await delay;
			}
			else
			{
				await Task.Yield();
			}

			throw ex;
		}

		[Test]
		public async Task AssertNotTimedOutTResultNormalExecution()
		{
			await Assert_NotTimedOut(Task.FromResult(true));
		}

		[Test]
		public async Task AssertNotTimedOutTResultErrorExecution()
		{
			await Assert_ThrowsAsync<CustomAssertionException>(async () =>
				await Assert_NotTimedOut(Task.Delay(10).ContinueWith(t => true), "task", Task.FromResult(true)));
		}

		[Test]
		public async Task AssertNotTimedOutTResultException()
		{
			await Assert_ThrowsAsync<Exception>(() =>
				Assert_NotTimedOut(Throw<bool>(new Exception("Error"))));
		}

		private async Task<TResult> Throw<TResult>(Exception ex, Task delay = null)
		{
			if (delay != null)
			{
				await delay;
			}
			else
			{
				await Task.Yield();
			}

			throw ex;
		}

		[Test]
		public async Task AssertTimedOutNormalExecution()
		{
			var task = Task.Delay(100);
			await Assert_TimedOut(task, "task", timeout: Task.FromResult(true));
		}

		[Test]
		public async Task AssertTimedOutErrorExecution()
		{
			await Assert_TimedOut(Throw(new Exception("Ignored"), Task.Delay(100)), "Throw", timeout: Task.FromResult(true));
		}

		[Test]
		public async Task AssertTimedOutErrorExecutionNotTimedOut()
		{
			await Assert_ThrowsAsync<CustomAssertionException>(async () =>
				await Assert_TimedOut(Throw(new Exception("Ignored"))));
		}

		protected Task<bool> AsyncOperation(bool throwException)
		{
			var tcs = new TaskCompletionSource<bool>();
			ThreadPool.QueueUserWorkItem(x =>
			{
				if (throwException)
				{
					tcs.TrySetException(new InvalidOperationException());
				}
				else
				{
					tcs.TrySetResult(true);
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

		[Test]
		public void ToStringTests()
		{
			var ss = new StubServer();
			var jc = new JsonClient(new StubClient(ss), new StubMessageTypeProvider(), new Serializer()) { DebugName = "MyClient" };
			Assert.AreEqual("MyClient", jc.ToString());

			var id = jc.GenerateMessageId();
			Assert.AreEqual("MyClient1", id);

			var pm = new JsonClient.PendingMessage { Name = "Test" };
			Assert.AreEqual("Test", pm.ToString());
		}

		[Test]
		public void TaskResultCanBeAccessedUsingDynamicVariable()
		{
			var task1 = Task.FromResult("This is a result of Task<string>");
			Task task2 = task1; // base Task type doesn't have the Result property
			Assert.IsTrue(task2.GetType().IsGenericType);

			// note: dynamics require Microsoft.CSharp and System.Dynamic.Runtime dependencies
			object result = ((dynamic)task2).Result;
			Assert.AreEqual(result, task1.Result);
		}

		[Test]
		public void TaskResultCanBeAccessedThroughReflection()
		{
			// doesn't work for open generic type:
			// var resultProperty = typeof(Task<>).GetProperty("Result");
			var task1 = Task.FromResult("Hello there");
			var resultProperty = task1.GetType().GetProperty("Result");
			Task task2 = task1;
			var result = resultProperty.GetValue(task2);
			Assert.AreEqual(task1.Result, result);
		}
	}
}
