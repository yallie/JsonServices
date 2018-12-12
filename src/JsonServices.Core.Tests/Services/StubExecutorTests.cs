using System;
using System.Threading.Tasks;
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

		private async Task<string> Execute(GetVersion msg)
		{
			var result = await (Executor.Execute(GetVersionName, msg) as Task<GetVersionResponse>);
			return result.Version;
		}

		[Test]
		public async Task StubExecutorExecutesGetVersionService()
		{
			Assert.ThrowsAsync<ArgumentNullException>(async () => await Execute(null));
			Assert.AreEqual("Version 0.01-alpha, build 12345, by yallie", await Execute(new GetVersion { IsInternal = true }));
			Assert.AreEqual("0.01-alpha", await Execute(new GetVersion { IsInternal = false }));
		}
	}
}
