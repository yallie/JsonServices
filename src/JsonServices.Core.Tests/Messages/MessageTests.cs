using JsonServices.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Messages
{
	[TestFixture]
	public class MessageTests
	{
		[Test]
		public void RequestMessageTests()
		{
			Assert.AreEqual("--> Something", new RequestMessage { Name = "Something" }.ToString());
			Assert.AreEqual("--> Something #2", new RequestMessage { Name = "Something", Id = "2" }.ToString());
		}

		[Test]
		public void ResponseMessageTests()
		{
			Assert.AreEqual("<-- Ok: Something", ResponseMessage.Create("Something", null, null).ToString());
			Assert.AreEqual("<-- Ok: Something #3", ResponseMessage.Create("Something", null, "3").ToString());
			Assert.AreEqual("<-- Error: Too bad (123)", ResponseMessage.Create(null, new Error { Code = 123, Message = "Too bad" }, null).ToString());
		}
	}
}
