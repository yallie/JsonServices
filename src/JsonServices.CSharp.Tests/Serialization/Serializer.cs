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

		public string SerializeRequest(RequestMessage message)
		{
			using (var config = JsConfig.BeginScope())
			{
				config.IncludeTypeInfo = false;
				config.ExcludeTypeInfo = true;
				return JsonSerializer.SerializeToString(message);
			}
		}

		public string SerializeResponse(ResponseMessage message)
		{
			using (var config = JsConfig.BeginScope())
			{
				config.IncludeTypeInfo = false;
				config.ExcludeTypeInfo = true;
				return JsonSerializer.SerializeToString(message);
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

		public RequestMessage DeserializeRequest(string data)
		{
			// pre-deserialize to get the message request type
			var msg = JsonSerializer.DeserializeFromString<RequestMessage>(data);
			var type = Locator.GetRequestType(msg.Name);
			var msgType = typeof(RequestMsg<>).MakeGenericType(new[] { type });

			// deserialize the strong-typed message
			var reqMsg = (IRequestMessage)JsonSerializer.DeserializeFromString(data, msgType);
			msg.Parameters = reqMsg.Parameters;
			return msg;
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

		public ResponseMessage DeserializeResponse(string name, string data)
		{
			// pre-deserialize to get the bulk of the message
			var msg = JsonSerializer.DeserializeFromString<ResponseMessage>(data);
			var type = Locator.GetResponseType(name);
			var msgType = typeof(ResponseMsg<>).MakeGenericType(new[] { type });

			// deserialize the strong-typed message
			var respMsg = (IResponseMessage)JsonSerializer.DeserializeFromString(data, msgType);
			msg.Result = respMsg.Result;
			return msg;
		}
	}
}
