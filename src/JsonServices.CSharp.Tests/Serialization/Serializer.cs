using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using ServiceStack.Text;

namespace JsonServices.Tests.Serialization
{
	public class Serializer : ISerializer
	{
		public Serializer(IMessageTypeLocator locator)
		{
			Locator = locator ?? throw new ArgumentNullException(nameof(locator));
		}

		private IMessageTypeLocator Locator { get; }

		public byte[] SerializeRequest(RequestMessage message)
		{
			using (var ms = new MemoryStream())
			{
				using (var config = JsConfig.BeginScope())
				{
					config.IncludeTypeInfo = false;
					config.ExcludeTypeInfo = true;
					JsonSerializer.SerializeToStream(message, ms);
				}

				return ms.ToArray();
			}
		}

		public byte[] SerializeResponse(ResponseMessage message)
		{
			using (var ms = new MemoryStream())
			{
				using (var config = JsConfig.BeginScope())
				{
					config.IncludeTypeInfo = false;
					config.ExcludeTypeInfo = true;
					JsonSerializer.SerializeToStream(message, ms);
				}

				return ms.ToArray();
			}
		}

		private interface IRequestMessage
		{
			object Parameters { get; }
		}

		[DataContract]
		public class RequestMsg<T> : IRequestMessage
		{
			[DataMember(Name = "params")]
			public T Parameters { get; set; }
			object IRequestMessage.Parameters => Parameters;
		}

		public RequestMessage DeserializeRequest(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			{
				// pre-deserialize to get the message request type
				var msg = JsonSerializer.DeserializeFromStream<RequestMessage>(ms);
				var type = Locator.GetRequestType(msg.Name);
				var msgType = typeof(RequestMsg<>).MakeGenericType(new[] { type });

				// rewind the stream and deserialize the strong-typed message
				ms.Position = 0;
				var reqMsg = (IRequestMessage)JsonSerializer.DeserializeFromStream(msgType, ms);
				msg.Parameters = reqMsg.Parameters;
				return msg;
			}
		}

		private interface IResponseMessage
		{
			object Result { get; }
		}

		[DataContract]
		public class ResponseMsg<T> : IResponseMessage
		{
			[DataMember(Name = "result")]
			public T Result { get; set; }
			object IResponseMessage.Result => Result;
		}

		public ResponseMessage DeserializeResponse(string name, byte[] data)
		{
			using (var ms = new MemoryStream(data))
			{
				// pre-deserialize to get the bulk of the message
				var msg = JsonSerializer.DeserializeFromStream<ResponseMessage>(ms);
				var type = Locator.GetResponseType(name);
				var msgType = typeof(ResponseMsg<>).MakeGenericType(new[] { type });

				// rewind the stream and deserialize the strong-typed message
				ms.Position = 0;
				var respMsg = (IResponseMessage)JsonSerializer.DeserializeFromStream(msgType, ms);
				msg.Result = respMsg.Result;
				return msg;
			}
		}
	}
}
