using System;
using JsonServices.Exceptions;
using JsonServices.Services;
using JsonServices.Tests.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Services
{
	[TestFixture]
	public class StubExecutorTests
	{
		private IServiceExecutor Executor { get; } = new StubExecutor(authenticationRequired: false);

		private string GetVersionName { get; } = typeof(GetVersion).FullName;

		[Test]
		public void UnknownServiceNameThrowsAnException()
		{
			Assert.Throws<MethodNotFoundException>(() => Executor.Execute("Foo", null));
		}

		[Test]
		public void StubExecutorExecutesGetVersionService()
		{
			Assert.Throws<ArgumentNullException>(() => Executor.Execute(GetVersionName, null));
			Assert.AreEqual("Version 0.01-alpha, build 12345, by yallie",
				(Executor.Execute(GetVersionName, new GetVersion { IsInternal = true }) as GetVersionResponse).Version);
			Assert.AreEqual("0.01-alpha",
				(Executor.Execute(GetVersionName, new GetVersion { IsInternal = false }) as GetVersionResponse).Version);
		}
	}
}
