using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Tests.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Services
{
	[TestFixture]
	public class StubLocatorTests
	{
		[Test]
		public void StubLocatorThrowsOnUnknownType()
		{
			Assert.Throws<InvalidOperationException>(() => new StubLocator().GetRequestType("Foo"));
			Assert.Throws<InvalidOperationException>(() => new StubLocator().GetResponseType("Bar"));
		}

		[Test]
		public void StubLocatorReturnsGetVersionRequestAndResponse()
		{
			var locator = new StubLocator();
			var reqType = locator.GetRequestType("JsonServices.Tests.Messages.GetVersion");
			Assert.IsNotNull(reqType);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersion", reqType.FullName);

			var respType = locator.GetResponseType("JsonServices.Tests.Messages.GetVersion");
			Assert.IsNotNull(respType);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersionResponse", respType.FullName);
		}
	}
}
