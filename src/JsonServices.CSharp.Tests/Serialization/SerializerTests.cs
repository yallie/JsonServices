using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public class SerializerTests
	{
		private ISerializer Serializer { get; } = new Serializer(new StubLocator());

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

			var payload = Serializer.SerializeRequest(msg);
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

			var payload = Serializer.SerializeRequest(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true},\"id\":\"123\"}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseOneWayMessage()
		{
			var msg = new ResponseMessage
			{
				Result = new GetVersionResponse
				{
					Version = "1.2.3.4"
				}
			};

			var payload = Serializer.SerializeResponse(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"}}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseMessage()
		{
			var msg = new ResponseMessage
			{
				Id = "321",
				Result = new GetVersionResponse
				{
					Version = "1.2.3.4"
				}
			};

			var payload = Serializer.SerializeResponse(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"},\"id\":\"321\"}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseOneWayMessageWithError()
		{
			var msg = new ResponseMessage
			{
				Error = new Error
				{
					Code = -3123,
					Message = "Something is rotten"
				}
			};

			var payload = Serializer.SerializeResponse(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"}}", payload);
		}

		[Test]
		public void SerializerCanSerializeResponseMessageWithError()
		{
			var msg = new ResponseMessage
			{
				Id = "112",
				Error = new Error
				{
					Code = -3123,
					Message = "Something is rotten"
				}
			};

			var payload = Serializer.SerializeResponse(msg);
			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"},\"id\":\"112\"}", payload);
		}

		[Test]
		public void SerializerCanDeserializeRequestOneWayMessage()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true}}";
			var msg = Serializer.DeserializeRequest(data);

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersion", msg.Name);
			Assert.NotNull(msg.Parameters);
			Assert.AreEqual(true, (msg.Parameters as GetVersion).IsInternal);
			Assert.IsNull(msg.Id);
		}

		[Test]
		public void SerializerCanDeserializeRequestMessage()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true},\"id\":\"123\"}";
			var msg = Serializer.DeserializeRequest(data);

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.AreEqual("JsonServices.Tests.Messages.GetVersion", msg.Name);
			Assert.NotNull(msg.Parameters);
			Assert.AreEqual(true, (msg.Parameters as GetVersion).IsInternal);
			Assert.AreEqual("123", msg.Id);
		}

		[Test]
		public void SerializerCanDeserializeResponseOneWayMessage()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"}}";
			var msg = Serializer.DeserializeResponse(data, id => "JsonServices.Tests.Messages.GetVersion");

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Error);
			Assert.NotNull(msg.Result);
			Assert.AreEqual("1.2.3.4", (msg.Result as GetVersionResponse).Version);
			Assert.IsNull(msg.Id);
		}

		[Test]
		public void SerializerCanDeserializeResponseMessage()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"result\":{\"Version\":\"1.2.3.4\"},\"id\":\"312\"}";
			var msg = Serializer.DeserializeResponse(data, id => "JsonServices.Tests.Messages.GetVersion");

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Error);
			Assert.NotNull(msg.Result);
			Assert.AreEqual("1.2.3.4", (msg.Result as GetVersionResponse).Version);
			Assert.AreEqual("312", msg.Id);
		}

		[Test]
		public void SerializerCanDeserializeResponseOneWayMessageWithError()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"}}";
			var msg = Serializer.DeserializeResponse(data, id => "JsonServices.Tests.Messages.GetVersion");

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Result);
			Assert.NotNull(msg.Error);
			Assert.AreEqual(-3123, msg.Error.Code);
			Assert.AreEqual("Something is rotten", msg.Error.Message);
			Assert.IsNull(msg.Error.Data);
			Assert.IsNull(msg.Id);
		}

		[Test]
		public void SerializerCanDeserializeResponseMessageWithError()
		{
			var data = "{\"jsonrpc\":\"2.0\",\"error\":{\"code\":-3123,\"message\":\"Something is rotten\"},\"id\":\"332\"}";
			var msg = Serializer.DeserializeResponse(data, id => "JsonServices.Tests.Messages.GetVersion");

			Assert.NotNull(msg);
			Assert.AreEqual("2.0", msg.Version);
			Assert.IsNull(msg.Result);
			Assert.NotNull(msg.Error);
			Assert.AreEqual(-3123, msg.Error.Code);
			Assert.AreEqual("Something is rotten", msg.Error.Message);
			Assert.IsNull(msg.Error.Data);
			Assert.AreEqual("332", msg.Id);
		}
	}
}
