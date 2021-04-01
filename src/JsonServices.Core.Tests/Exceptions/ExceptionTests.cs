using System;
using JsonServices.Exceptions;
using JsonServices.Messages;
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
				MessageId = "MsgId",
				Details = "Divide by zero or whatever",
			};

			Assert.AreEqual(123, ex.Code);
			Assert.AreEqual("Hello", ex.Message);
			Assert.AreEqual("MsgId", ex.MessageId);
			Assert.AreEqual("Divide by zero or whatever", ex.Details);
		}

		[Test]
		public void ExceptionsHaveStandardCodesAndMessages()
		{
			Test<JsonServicesException>(0, nameof(JsonServicesException), "123");
			Test<AuthFailedException>(AuthFailedException.ErrorCode, "Authentication failed", "3123123");
			Test<AuthRequiredException>(AuthRequiredException.ErrorCode, "Authentication is required: test", "456456");
			Test<ClientDisconnectedException>(ClientDisconnectedException.ErrorCode, "Client is disconnected", "315435");
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

		[Test]
		public void GetExceptionTypesReturnsAllExceptionTypes()
		{
			var types = JsonServicesException.GetExceptionTypes();
			Assert.AreEqual(typeof(AuthFailedException), types[AuthFailedException.ErrorCode]);
			Assert.AreEqual(typeof(AuthRequiredException), types[AuthRequiredException.ErrorCode]);
			Assert.AreEqual(typeof(ClientDisconnectedException), types[ClientDisconnectedException.ErrorCode]);
			Assert.AreEqual(typeof(InternalErrorException), types[InternalErrorException.ErrorCode]);
			Assert.AreEqual(typeof(InvalidRequestException), types[InvalidRequestException.ErrorCode]);
			Assert.AreEqual(typeof(MethodNotFoundException), types[MethodNotFoundException.ErrorCode]);
			Assert.AreEqual(typeof(ParseErrorException), types[ParseErrorException.ErrorCode]);
		}

		[Test]
		public void CreateExceptionsFromErrors()
		{
			Roundtrip<JsonServicesException>();
			Roundtrip<AuthFailedException>();
			Roundtrip<AuthRequiredException>();
			Roundtrip<ClientDisconnectedException>();
			Roundtrip<InternalErrorException>();
			Roundtrip<InvalidRequestException>();
			Roundtrip<MethodNotFoundException>();
			Roundtrip<ParseErrorException>();
		}

		private void Roundtrip<TException>()
			where TException : JsonServicesException
		{
			// source exception
			var src = Activator.CreateInstance(typeof(TException), nonPublic: true) as TException;
			src.Details = Guid.NewGuid().ToString();
			src.MessageId = Guid.NewGuid().ToString();

			// Exception -> JSON-RPC Error
			var error = new Error
			{
				Code = src.Code,
				Message = src.Message,
				Data = src.Details,
			};

			// JSON-RPC Error -> Exception
			var nex = JsonServicesException.Create(error, src.MessageId);
			Assert.AreSame(src.GetType(), nex.GetType());
			Assert.AreEqual(src.Code, nex.Code);
			Assert.AreEqual(src.Message, nex.Message);
			Assert.AreEqual(src.MessageId, nex.MessageId);
			Assert.AreEqual(src.Details, nex.Details);
		}

		[Test]
		public void NonSerializableErrorDataShouldntThrow()
		{
			Assert.DoesNotThrow(() =>
			{
				var error = new Error
				{
					Code = AuthFailedException.ErrorCode,
					Message = "As the dust settles, see our dreams, All coming true, It depends on you",
					Data = new
					{
						Message = "I'm not serializable!"
					},
				};

				var ex = JsonServicesException.Create(error);
			});
		}
	}
}
