using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Services;
using JsonServices.Tests.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Services
{
	[TestFixture]
	public class StubExecutorTests
	{
		private IServiceExecutor Executor { get; } = new StubExecutor();

		private string GetVersionName { get; } = typeof(GetVersion).FullName;

		[Test]
		public void UnknownServiceNameThrowsAnException()
		{
			Assert.Throws<InvalidOperationException>(() => Executor.Execute("Foo", null));
		}

		[Test]
		public void StubExecutorExecutesGetVersionService()
		{
			Assert.Throws<ArgumentNullException>(() => Executor.Execute(GetVersionName, null));
			Assert.AreEqual("Version 0.01-alpha, build 12345, by yallie", Executor.Execute(GetVersionName, new GetVersion { IsInternal = true }));
			Assert.AreEqual("0.01-alpha", Executor.Execute(GetVersionName, new GetVersion { IsInternal = false }));
		}
	}
}
