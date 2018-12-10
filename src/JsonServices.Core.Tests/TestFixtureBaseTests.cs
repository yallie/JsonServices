using System;
using System.Threading;
using System.Threading.Tasks;
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
