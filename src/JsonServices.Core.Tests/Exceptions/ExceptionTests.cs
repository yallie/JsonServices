using System;
using JsonServices.Exceptions;
using NUnit.Framework;

namespace JsonServices.Tests.Exceptions
{
	[TestFixture]
	public class ExceptionTests
	{
		[Test]
		public void JsonServicesExceptionHasCodeMessageAndMessageId()
		{
			var ex = new JsonServicesException(123, "Hello")
			{
				MessageId = "MsgId"
			};

			Assert.AreEqual(123, ex.Code);
			Assert.AreEqual("Hello", ex.Message);
			Assert.AreEqual("MsgId", ex.MessageId);
		}

		[Test]
		public void ExceptionsHaveStandardCodesAndMessages()
		{
			Test<AuthFailedException>(AuthFailedException.ErrorCode, "Authentication failed", "3123123");
			Test<AuthRequiredException>(AuthRequiredException.ErrorCode, "Authentication is required: test", "456456");
			Test<InternalErrorException>(InternalErrorException.ErrorCode, "Internal error: test", "56657");
			Test<InvalidRequestException>(InvalidRequestException.ErrorCode, "Invalid request. Request data: test", "56657");
			Test<MethodNotFoundException>(MethodNotFoundException.ErrorCode, "Method not found: test", "7897897");
			Test<ParseErrorException>(ParseErrorException.ErrorCode, "Parse error. Message data: test", "890890");
		}

		private void Test<TException>(int code, string message, string messageId)
			where TException : JsonServicesException
		{
			var ex = Activator.CreateInstance(typeof(TException), nonPublic: true) as TException;
			ex.MessageId = messageId;

			Assert.AreEqual(code, ex.Code);
			Assert.AreEqual(message, ex.Message);
			Assert.AreEqual(messageId, ex.MessageId);
		}
	}
}
