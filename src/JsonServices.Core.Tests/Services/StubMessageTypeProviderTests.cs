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
			var reqType = locator.GetRequestType("JsonServices.Tests.Messages.GetVersion");
			Assert.IsNotNull(reqType);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersion", reqType.FullName);

			var respType = locator.GetResponseType("JsonServices.Tests.Messages.GetVersion");
			Assert.IsNotNull(respType);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersionResponse", respType.FullName);
		}
	}
}
