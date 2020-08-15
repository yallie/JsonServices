using System;
using System.Globalization;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public abstract class SerializerTestsBase
	{
		protected abstract ISerializer Serializer { get; }

		private IMessageTypeProvider TypeProvider { get; } = new StubMessageTypeProvider();

		private IMessageNameProvider NameProvider { get; } = new StubMessageNameProvider(typeof(GetVersion).FullName);

		[Test]
		public void SerializerCanSerializeRequestOneWayMessage()
		{
			var msg = new RequestMessage
			{
				Name = typeof(GetVersion).FullName,
				Parameters = new GetVersion
				{
					IsInternal = true
				}
			};

			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true}}", payload);
		}

		[Test]
		public void SerializerCanSerializeRequestMessage()
		{
			var msg = new RequestMessage
			{
				Id = "123",
				Name = typeof(GetVersion).FullName,
				Parameters = new GetVersion
				{
					IsInternal = true
				}
			};

			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true},\"id\":\"123\"}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseOneWayMessage()
		{
			var msg = new ResponseResultMessage
			{
				Result = new GetVersionResponse
				{
					Version = "1.2.3.4"
				}
			};

			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"}}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseMessage()
		{
			var msg = new ResponseResultMessage
			{
				Id = "321",
				Result = new GetVersionResponse
				{
					Version = "1.2.3.4"
				}
			};

			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"},\"id\":\"321\"}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseOneWayMessageWithNullResult()
		{
			var msg = new ResponseResultMessage();
			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"result\":null}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseMessageWithNullResult()
		{
			var msg = new ResponseResultMessage { Id = "111" };
			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"result\":null,\"id\":\"111\"}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseOneWayMessageWithError()
		{
			var msg = new ResponseErrorMessage
			{
				Error = new Error
				{
					Code = -3123,
					Message = "Something is rotten"
				}
			};

			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.That(payload, Is.AnyOf(
				"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"}}",
				"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\",\"data\":null}}"));
		}

		[Test]
		public void SerializerCanSerializeResponseMessageWithError()
		{
			var msg = new ResponseErrorMessage
			{
				Id = "112",
				Error = new Error
				{
					Code = -3123,
					Message = "Something is rotten"
				}
			};

			var payload = Serializer.Serialize(msg);
			Assert.NotNull(payload);
			Assert.That(payload, Is.AnyOf(
				"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"},\"id\":\"112\"}",
				"{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\",\"data\":null},\"id\":\"112\"}"));
		}

		[Test]
		public void SerializerCanDeserializeRequestOneWayMessage()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true}}";
			var msg = Serializer.Deserialize(data, TypeProvider, NameProvider) as RequestMessage;

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersion", msg.Name);
			Assert.NotNull(msg.Parameters);
			Assert.AreEqual(true, (msg.Parameters as GetVersion).IsInternal);
			Assert.IsNull(msg.Id);
		}

		[Test]
		public void SerializerThrowsInvalidRequestExceptionOnInvalidMessage()
		{
			var ex = Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize("bozo", TypeProvider, NameProvider));
			Assert.IsNull(ex.MessageId);
		}

		[Test]
		public void SerializerThrowsInvalidRequestExceptionOnNullMessage()
		{
			var ex = Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize("null", TypeProvider, NameProvider));
			Assert.IsNull(ex.MessageId);
		}

		[Test]
		public void SerializerThrowsInvalidRequestExceptionOnEmptyMessage()
		{
			var ex = Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize(string.Empty, TypeProvider, NameProvider));
			Assert.IsNull(ex.MessageId);
		}

		[Test]
		public void SerializerThrowsInvalidRequestExceptionOnNullString()
		{
			var ex = Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize(null, TypeProvider, NameProvider));
			Assert.IsNull(ex.MessageId);
		}

		[Test]
		public void SerializerThrowsInvalidRequestExceptionWhenMessageTypeProviderThrowsAnUnhandledException()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true},\"id\":\"123\"}";
			var ex = Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize(data, new BrokenMessageTypeProvider(), NameProvider));
			Assert.AreEqual("123", ex.MessageId);
		}

		[Test]
		public void SerializerCanDeserializeRequestMessage()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true},\"id\":\"123\"}";
			var msg = Serializer.Deserialize(data, TypeProvider, NameProvider) as RequestMessage;

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersion", msg.Name);
			Assert.NotNull(msg.Parameters);
			Assert.AreEqual(true, (msg.Parameters as GetVersion).IsInternal);
			Assert.AreEqual("123", msg.Id);
		}

		[Test]
		public void SerializerCannotDeserializeResponseOneWayMessage()
		{
			// missing id, cannot determine what's that message was sent for
			var data = "{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"}}";
			Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize(data, TypeProvider, NameProvider));
		}

		[Test]
		public void SerializerCanDeserializeResponseMessage()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"},\"id\":\"312\"}";
			var msg = Serializer.Deserialize(data, TypeProvider, NameProvider) as ResponseMessage;

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Error);
			Assert.NotNull(msg.Result);
			Assert.AreEqual("1.2.3.4", (msg.Result as GetVersionResponse).Version);
			Assert.AreEqual("312", msg.Id);
		}

		[Test]
		public void SerializerCannotDeserializeResponseOneWayMessageWithError()
		{
			// missing id, cannot determine what's that message was sent for
			var data = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"}}";
			Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize(data, TypeProvider, NameProvider));
		}

		[Test]
		public void SerializerCanDeserializeResponseMessageWithError()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"},\"id\":\"332\"}";
			var msg = Serializer.Deserialize(data, TypeProvider, NameProvider) as ResponseMessage;

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Result);
			Assert.NotNull(msg.Error);
			Assert.AreEqual(-3123, msg.Error.Code);
			Assert.AreEqual("Something is rotten", msg.Error.Message);
			Assert.IsNull(msg.Error.Data);
			Assert.AreEqual("332", msg.Id);
		}

		[Test]
		public void SerializerCanDeserializeResponseMessageOfTypeVoid()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"result\":{\"code\":-3123,\"message\":\"Something is rotten\"},\"id\":\"332\"}";
			var msg = Serializer.Deserialize(data, TypeProvider, new StubMessageNameProvider(typeof(EventBroadcaster).FullName)) as ResponseMessage;

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Result);
			Assert.IsNull(msg.Error);
			Assert.AreEqual("332", msg.Id);
		}

		[Test]
		public void SerializerCanDeserializeResponseMessageOfTypeVoidWithError()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"},\"id\":\"332\"}";
			var msg = Serializer.Deserialize(data, TypeProvider, new StubMessageNameProvider(typeof(EventBroadcaster).FullName)) as ResponseMessage;

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Result);
			Assert.NotNull(msg.Error);
			Assert.AreEqual(-3123, msg.Error.Code);
			Assert.AreEqual("Something is rotten", msg.Error.Message);
			Assert.IsNull(msg.Error.Data);
			Assert.AreEqual("332", msg.Id);
		}

		[Test]
		public void SerializerReportsMessageIdWhenMessageTypeIsUnrecognized()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"method\":\"Unknown\",\"params\":{\"IsInternal\":true},\"id\":\"23432423\"}";
			var ex = Assert.Throws<MethodNotFoundException>(() => Serializer.Deserialize(data, TypeProvider, NameProvider));

			Assert.NotNull(ex);
			Assert.AreEqual("23432423", ex.MessageId);
		}

		[Test]
		public void SerializerUsesJavascriptCompatibleDateFormats()
		{
			var msg = new RequestMessage
			{
				Name = "Date",
				Parameters = new
				{
					Date = new DateTime(2018, 12, 16, 8, 37, 30),
				},
			};

			var serialized = Serializer.Serialize(msg);
			Assert.NotNull(serialized);

			// ServiceStack adds fractional parts of a second, so we can't use Assert.AreEqual
			// Newtonsoft:   Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"Date\",\"params\":{\"Date\":\"2018-12-16T08:37:30\"}}", serialized);
			// ServiceStack: Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"Date\",\"params\":{\"Date\":\"2018-12-16T08:37:30.0000000\"}}", serialized);
			Assert.IsTrue(serialized.StartsWith("{\"jsonrpc\":\"2.0\",\"method\":\"Date\",\"params\":{\"Date\":\"2018-12-16T08:37:30"));
			Assert.IsTrue(serialized.EndsWith("0\"}}"));
			Assert.IsTrue(serialized.Length <= 81);
		}

		[Test]
		public void SerializerCanSerializeRequestMessagesWithSimpleTuples()
		{
			var msg = new RequestMessage
			{
				Name = "SimpleTuple",
				Parameters = Tuple.Create("a", 1, true, 2.34m, 'c'),
			};

			var serialized = Serializer.Serialize(msg);
			Assert.NotNull(serialized);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"SimpleTuple\",\"params\":{\"Item1\":\"a\",\"Item2\":1,\"Item3\":true,\"Item4\":2.34,\"Item5\":\"c\"}}", serialized);
		}

		[Test]
		public void SerializerCanDeserializeRequestMessagesWithSimpleTuples()
		{
			var provider = new MessageTypeProvider();
			provider.Register("SimpleTuple", typeof(Tuple<string, int, bool, decimal, char>));
			var serialized = "{\"jsonrpc\":\"2.0\",\"method\":\"SimpleTuple\",\"params\":{\"Item1\":\"a\",\"Item2\":1,\"Item3\":true,\"Item4\":2.34,\"Item5\":\"c\"}}";

			var msg = Serializer.Deserialize(serialized, provider, null) as RequestMessage;
			Assert.NotNull(msg);
			Assert.AreEqual("SimpleTuple", msg.Name);
			Assert.AreEqual(Tuple.Create("a", 1, true, 2.34m, 'c'), msg.Parameters);
		}

		[Test]
		public void SerializerCanSerializeRequestMessagesWithArraysOfSimpleValues()
		{
			var msg = new RequestMessage
			{
				Name = "SimpleArray",
				Parameters = new object[] { "a", 1, true, 2.34m, 'c' },
			};

			var serialized = Serializer.Serialize(msg);
			Assert.NotNull(serialized);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"SimpleArray\",\"params\":[\"a\",1,true,2.34,\"c\"]}", serialized);
		}

		[Test]
		public void SerializerCanDeserializeRequestMessagesWithArraysOfSimpleValues()
		{
			var provider = new MessageTypeProvider();
			provider.Register("SimpleArray", typeof(object[]));
			var serialized = "{\"jsonrpc\":\"2.0\",\"method\":\"SimpleArray\",\"params\":[\"a\",1,true,2.34,\"c\"]}";

			var msg = Serializer.Deserialize(serialized, provider, null) as RequestMessage;
			Assert.NotNull(msg);
			Assert.AreEqual("SimpleArray", msg.Name);

			// Newtonsoft parses primitive types
			// ServiceStack parses all values as strings
			// System.Text.Json returns all values as JsonElement instances
			// either way, we can't have chars and decimals, too bad
			var array = msg.Parameters as object[];
			Assert.AreEqual("a", array[0]);
			Assert.AreEqual("1", array[1].ToString());
			Assert.AreEqual("true", array[2].ToString().ToLower());
			Assert.AreEqual("2.34", array[3].ToString().Replace(",", "."));
			Assert.AreEqual("c", array[4].ToString());
		}

		[Test]
		public void SerializerOnTheServersSideCannotHandleResponseMessage()
		{
			// missing MessageNameProvider, cannot determine what's that message was sent for
			var data = "{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"},\"id\":\"1\"}";
			Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize(data, TypeProvider, null));
		}

		[Test]
		public void SerializerThrowsInvalidRequestWhenPendingRequestIsNotFound()
		{
			// MessageNameProvider returns null, cannot determine what's that message was sent for
			var data = "{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"},\"id\":\"1\"}";
			Assert.Throws<InvalidRequestException>(() => Serializer.Deserialize(data, TypeProvider, new StubMessageNameProvider(null)));
		}

		[Test]
		public void SerializedCultureInfoDoesntHaveAllItsParentsIncluded()
		{
			var msg = new ResponseResultMessage
			{
				Id = "1",
				Result = CultureInfo.GetCultureInfo("en-US"),
			};

			var serialized = Serializer.Serialize(msg);
			Assert.NotNull(serialized);
			Assert.IsFalse(serialized.Contains("{\"Parent\":{\"Parent\":{\"Parent\""));
		}

		[Test]
		public void SerializesOutputsGuidsWithDashes()
		{
			var msg = new RequestMessage
			{
				Parameters = Guid.Parse("db22d085-3c9c-4df3-a5ba-2276a47372e3"),
			};

			var serialized = Serializer.Serialize(msg);
			Assert.NotNull(serialized);
			Assert.IsTrue(serialized.Contains("db22d085-3c9c-4df3-a5ba-2276a47372e3"));
		}
	}
}
