using System;
using System.IO;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization.Newtonsoft.Internal;
using JsonServices.Services;
using Newtonsoft.Json;

namespace JsonServices.Serialization.Newtonsoft
{
	public class Serializer : ISerializer
	{
		public Serializer(IMessageTypeProvider typeProvider, IMessageNameProvider nameProvider = null)
		{
			MessageTypeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
			MessageNameProvider = nameProvider;
			JsonSerializer = JsonSerializer.Create();
		}

		private JsonSerializer JsonSerializer { get; set; }

		public IMessageTypeProvider MessageTypeProvider { get; }

		public IMessageNameProvider MessageNameProvider { get; set; }

		public string Serialize(IMessage message)
		{
			using (var sw = new StringWriter())
			{
				JsonSerializer.Serialize(sw, message);
				return sw.ToString();
			}
		}

		public IMessage Deserialize(string data)
		{
			using (var sr = new StringReader(data))
			{
				// validate message, detect message type and name
				var preview = (GenericMessage)JsonSerializer.Deserialize(sr, typeof(GenericMessage));
				if (!preview.IsValid)
				{
					throw new InvalidRequestException(data);
				}

				// detect message name
				var name = preview.Name;
				var isRequest = name != null;
				if (name == null)
				{
					// server cannot handle a response message
					if (MessageNameProvider == null)
					{
						throw new InvalidRequestException(data);
					}

					// invalid request id
					name = MessageNameProvider.GetMessageName(preview.Id);
					if (name == null)
					{
						throw new InvalidRequestException(name);
					}
				}

				// deserialize request or response message
				if (isRequest)
				{
					return DeserializeRequest(data, name, preview.Id);
				}

				return DeserializeResponse(data, name, preview.Id, preview.Error);
			}
		}

		private RequestMessage DeserializeRequest(string data, string name, string id)
		{
			using (var sr = new StringReader(data))
			{
				// get the message request type
				var type = MessageTypeProvider.GetRequestType(name);
				var msgType = typeof(RequestMsg<>).MakeGenericType(new[] { type });

				// deserialize the strong-typed message
				var reqMsg = (IRequestMessage)JsonSerializer.Deserialize(sr, msgType);
				return new RequestMessage
				{
					Name = name,
					Parameters = reqMsg.Parameters,
					Id = id,
				};
			}
		}

		public ResponseMessage DeserializeResponse(string data, string name, string id, Error error)
		{
			using (var sr = new StringReader(data))
			{
				// pre-deserialize to get the bulk of the message
				var type = MessageTypeProvider.GetResponseType(name);
				var msgType = typeof(ResponseMsg<>).MakeGenericType(new[] { type });

				// deserialize the strong-typed message
				var respMsg = (IResponseMessage)JsonSerializer.Deserialize(sr, msgType);
				return new ResponseMessage
				{
					Result = respMsg.Result,
					Error = error,
					Id = id,
				};
			}
		}
	}
}
