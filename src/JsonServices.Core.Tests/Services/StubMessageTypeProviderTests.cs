using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Tests.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Services
{
	[TestFixture]
	public class StubMessageTypeProviderTests
	{
		[Test]
		public void StubLocatorThrowsOnUnknownType()
		{
			Assert.Throws<MethodNotFoundException>(() => new StubMessageTypeProvider().GetRequestType("Foo"));
			Assert.Throws<MethodNotFoundException>(() => new StubMessageTypeProvider().GetResponseType("Bar"));
		}

		[Test]
		public void StubLocatorReturnsGetVersionRequestAndResponse()
		{
			var locator = new StubMessageTypeProvider();
			var reqType = locator.GetRequestType(typeof(GetVersion).FullName);
			Assert.IsNotNull(reqType);
			Assert.AreEqual(typeof(GetVersion).FullName, reqType.FullName);

			var respType = locator.GetResponseType(typeof(GetVersion).FullName);
			Assert.IsNotNull(respType);
			Assert.AreEqual(typeof(GetVersionResponse).FullName, respType.FullName);
		}

		[Test]
		public void StubLocatorReturnsEventBroadcasterRequestAndResponse()
		{
			var locator = new StubMessageTypeProvider();
			var reqType = locator.GetRequestType(typeof(EventBroadcaster).FullName);
			Assert.IsNotNull(reqType);
			Assert.AreEqual(typeof(EventBroadcaster).FullName, reqType.FullName);

			var respType = locator.GetResponseType(typeof(EventBroadcaster).FullName);
			Assert.IsNotNull(respType);
			Assert.AreEqual(typeof(void).FullName, respType.FullName);
		}
	}
}
