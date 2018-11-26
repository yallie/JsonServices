using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;
using Newtonsoft.Json;

namespace JsonServices.Tests.Serialization.Newtonsoft.Json
{
	public class Serializer : ISerializer
	{
		public Serializer(IMessageTypeProvider provider)
		{
			MessageTypeProvider = provider ?? throw new ArgumentNullException(nameof(provider));
			JsonSerializer = JsonSerializer.Create();
		}

		private JsonSerializer JsonSerializer { get; set; }

		public IMessageTypeProvider MessageTypeProvider { get; }

		public string SerializeRequest(RequestMessage message)
		{
			using (var sw = new StringWriter())
			{
				JsonSerializer.Serialize(sw, message);
				return sw.ToString();
			}
		}

		public string SerializeResponse(ResponseMessage message)
		{
			using (var sw = new StringWriter())
			{
				JsonSerializer.Serialize(sw, message);
				return sw.ToString();
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
			using (var sr = new StringReader(data))
			{
				// pre-deserialize to get the message request type
				var msg = (RequestMessage)JsonSerializer.Deserialize(sr, typeof(RequestMessage));
				var type = MessageTypeProvider.GetRequestType(msg.Name);
				var msgType = typeof(RequestMsg<>).MakeGenericType(new[] { type });

				// deserialize the strong-typed message
				using (var sr2 = new StringReader(data))
				{
					var reqMsg = (IRequestMessage)JsonSerializer.Deserialize(sr2, msgType);
					msg.Parameters = reqMsg.Parameters;
					return msg;
				}
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

		public ResponseMessage DeserializeResponse(string data, Func<string, string> getName)
		{
			using (var sr = new StringReader(data))
			{
				// pre-deserialize to get the bulk of the message
				var msg = (ResponseMessage)JsonSerializer.Deserialize(sr, typeof(ResponseMessage));
				var name = getName(msg.Id);
				var type = MessageTypeProvider.GetResponseType(name);
				var msgType = typeof(ResponseMsg<>).MakeGenericType(new[] { type });

				// deserialize the strong-typed message
				using (var sr2 = new StringReader(data))
				{
					// deserialize the strong-typed message
					var respMsg = (IResponseMessage)JsonSerializer.Deserialize(sr2, msgType);
					msg.Result = respMsg.Result;
					return msg;
				}
			}
		}
	}
}
