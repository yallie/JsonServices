using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class TestFixtureBase
	{
		protected Task Timeout => Task.Delay(500);

		protected async Task<TResult> Assert_NotTimedOut<TResult>(Task<TResult> task, string code = null, Task timeout = null)
		{
			if (await Task.WhenAny(task, timeout ?? Timeout) != task)
			{
				Assert.Fail((code ?? "The given task") + " has timed out!");
			}

			return task.Result;
		}

		protected async Task Assert_NotTimedOut(Task task, string code = null, Task timeout = null)
		{
			if (await Task.WhenAny(task, timeout ?? Timeout) != task)
			{
				Assert.Fail((code ?? "The given task") + " has timed out!");
			}
		}

		protected async Task Assert_TimedOut(Task task, string code = null, Task timeout = null)
		{
			timeout = timeout ?? Timeout;
			if (await Task.WhenAny(task, timeout) != timeout)
			{
				Assert.Fail((code ?? "The given task") + " has not timed out as it should!");
			}
		}

		// awaitable version of Assert.ThrowsAsync
		protected async Task<T> Assert_ThrowsAsync<T>(AsyncTestDelegate code)
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
