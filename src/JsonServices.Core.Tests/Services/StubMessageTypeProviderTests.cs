using JsonServices.Exceptions;
using JsonServices.Services;
using JsonServices.Tests.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Services
{
	[TestFixture]
	public class StubMessageTypeProviderTests
	{
		[Test]
		public void StubMessageTypeProviderThrowsOnUnknownType()
		{
			Assert.Throws<MethodNotFoundException>(() => new StubMessageTypeProvider().GetRequestType("Foo"));
			Assert.Throws<MethodNotFoundException>(() => new StubMessageTypeProvider().GetResponseType("Bar"));
		}

		[Test]
		public void StubMessageTypeProviderReturnsGetVersionRequestAndResponse()
		{
			var provider = new StubMessageTypeProvider();
			var reqType = provider.GetRequestType(typeof(GetVersion).FullName);
			Assert.IsNotNull(reqType);
			Assert.AreEqual(typeof(GetVersion).FullName, reqType.FullName);

			var respType = provider.GetResponseType(typeof(GetVersion).FullName);
			Assert.IsNotNull(respType);
			Assert.AreEqual(typeof(GetVersionResponse).FullName, respType.FullName);
		}

		[Test]
		public void StubMessageTypeProviderReturnsEventBroadcasterRequestAndResponse()
		{
			var provider = new StubMessageTypeProvider();
			var reqType = provider.GetRequestType(typeof(EventBroadcaster).FullName);
			Assert.IsNotNull(reqType);
			Assert.AreEqual(typeof(EventBroadcaster).FullName, reqType.FullName);

			var respType = provider.GetResponseType(typeof(EventBroadcaster).FullName);
			Assert.IsNotNull(respType);
			Assert.AreEqual(typeof(void).FullName, respType.FullName);
		}

		[Test]
		public void StubMessageTypeProviderDoesntThrowIfMessageTypeDoesntImplementIReturn()
		{
			var provider = new StubMessageTypeProvider();
			provider.Register("Foo", typeof(object));
			Assert.AreEqual(typeof(object), provider.TryGetRequestType("Foo"));
			Assert.AreEqual(typeof(void), provider.TryGetResponseType("Foo"));
		}
	}
}
