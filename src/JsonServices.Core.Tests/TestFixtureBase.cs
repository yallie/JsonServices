using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class TestFixtureBase
	{
		protected Task Timeout => Task.Delay(500);

		protected async Task<TResult> Assert_NotTimedOut<TResult>(Task<TResult> task, string message = null, Task timeout = null)
		{
			if (await Task.WhenAny(task, timeout ?? Timeout) != task)
			{
				Assert.Fail(message ?? "The given task has timed out!");
			}

			return task.Result;
		}

		protected async Task Assert_NotTimedOut(Task task, string message = null, Task timeout = null)
		{
			if (await Task.WhenAny(task, timeout ?? Timeout) != task)
			{
				Assert.Fail(message ?? "The given task has timed out!");
			}
		}

		protected async Task Assert_TimedOut(Task task, string message = null, Task timeout = null)
		{
			timeout = timeout ?? Timeout;
			if (await Task.WhenAny(task, timeout) != timeout)
			{
				Assert.Fail(message ?? "The given task has not timed out as it should!");
			}
		}

	}
}
