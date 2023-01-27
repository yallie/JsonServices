using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JsonServices.Exceptions;
using JsonServices.Messages;
using NUnit.Framework;

namespace JsonServices.Serialization.Tests
{
	[TestFixture]
	public class SerializableTests
	{
		[Test]
		public void ExceptionsAreSerializable()
		{
			var error = new Error
			{
				Message = "unknown error",
				Data = "no data",
			};

			Roundtrip(() => new JsonServicesException(1, error.Message), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(1));
				Assert.That(ex.Message, Is.EqualTo(error.Message));
			});

			Roundtrip(() => new AuthFailedException(error), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(AuthFailedException.ErrorCode));
				Assert.That(ex.Message, Is.EqualTo("unknown error"));
				Assert.That(ex.Details, Is.EqualTo("no data"));
			});

			Roundtrip(() => new AuthRequiredException(error), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(AuthRequiredException.ErrorCode));
				Assert.That(ex.Message, Is.EqualTo("unknown error"));
				Assert.That(ex.Details, Is.EqualTo("no data"));
			});

			Roundtrip(() => new ClientDisconnectedException(error), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(ClientDisconnectedException.ErrorCode));
				Assert.That(ex.Message, Is.EqualTo("unknown error"));
				Assert.That(ex.Details, Is.EqualTo("no data"));
			});

			Roundtrip(() => new InternalErrorException(error), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(InternalErrorException.ErrorCode));
				Assert.That(ex.Message, Is.EqualTo("unknown error"));
				Assert.That(ex.Details, Is.EqualTo("no data"));
			});

			Roundtrip(() => new InvalidRequestException(error), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(InvalidRequestException.ErrorCode));
				Assert.That(ex.Message, Is.EqualTo("unknown error"));
				Assert.That(ex.Details, Is.EqualTo("no data"));
			});

			Roundtrip(() => new MethodNotFoundException(error), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(MethodNotFoundException.ErrorCode));
				Assert.That(ex.Message, Is.EqualTo("unknown error"));
				Assert.That(ex.Details, Is.EqualTo("no data"));
			});

			Roundtrip(() => new ParseErrorException(error), ex =>
			{
				Assert.That(ex.Code, Is.EqualTo(ParseErrorException.ErrorCode));
				Assert.That(ex.Message, Is.EqualTo("unknown error"));
				Assert.That(ex.Details, Is.EqualTo("no data"));
			});
		}

		private void Roundtrip<T>(Func<T> factory, Action<T> verifier)
			where T : JsonServicesException
		{
			var ex = factory();
			var fmt = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				fmt.Serialize(ms, ex);

				ms.Position = 0;
				
				var dx = fmt.Deserialize(ms) as T;
				Assert.That(dx, Is.Not.Null);
				verifier(dx);
			}
		}
	}
}
