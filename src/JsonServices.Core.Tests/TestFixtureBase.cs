using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class TestFixtureBase : IDisposable
	{
		protected Task Timeout => Task.Delay(1500);

		[Serializable]
		public class CustomAssertionException : Exception
		{
			public CustomAssertionException(string message) : base(message)
			{
			}
		}

		protected async Task<TResult> Assert_NotTimedOut<TResult>(Task<TResult> task, string code = null, Task timeout = null)
		{
			try
			{
				if (await Task.WhenAny(task, timeout ?? Timeout) != task)
				{
					throw new CustomAssertionException((code ?? "The given task") + " has timed out!");
				}

				return task.Result;
			}
			catch (AggregateException ex)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
				throw;
			}
		}

		protected async Task Assert_NotTimedOut(Task task, string code = null, Task timeout = null)
		{
			try
			{
				if (await Task.WhenAny(task, timeout ?? Timeout) != task)
				{
					throw new CustomAssertionException((code ?? "The given task") + " has timed out!");
				}

				await task;
			}
			catch (AggregateException ex)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
				throw;
			}
		}

		protected async Task Assert_TimedOut(Task task, string code = null, Task timeout = null)
		{
			try
			{
				timeout = timeout ?? Timeout;
				if (await Task.WhenAny(task, timeout) != timeout)
				{
					throw new CustomAssertionException((code ?? "The given task") + " has not timed out as it should!");
				}

				await timeout;
			}
			catch (AggregateException ex)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
				throw;
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

		public virtual void Dispose()
		{
			// handle per-fixture cleanup here
		}
	}
}
