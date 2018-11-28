using System;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization.ServiceStack.Internal;
using JsonServices.Services;
using ServiceStack.Text;

namespace JsonServices.Serialization.ServiceStack
{
	public class Serializer : ISerializer
	{
		public Serializer(IMessageTypeProvider provider, IMessageNameProvider nameProvider = null)
		{
			MessageTypeProvider = provider ?? throw new ArgumentNullException(nameof(provider));
			MessageNameProvider = nameProvider;
		}

		public IMessageTypeProvider MessageTypeProvider { get; }

		public IMessageNameProvider MessageNameProvider { get; set; }

		public string Serialize(IMessage message)
		{
			using (var config = JsConfig.BeginScope())
			{
				config.IncludeTypeInfo = false;
				config.ExcludeTypeInfo = true;
				return JsonSerializer.SerializeToString(message);
			}
		}

		public IMessage Deserialize(string data)
		{
			{
				// validate message, detect message type and name
				var preview = (GenericMessage)JsonSerializer.DeserializeFromString(data, typeof(GenericMessage));
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
			// pre-deserialize to get the message request type
			var type = MessageTypeProvider.GetRequestType(name);
			var msgType = typeof(RequestMsg<>).MakeGenericType(new[] { type });

			// deserialize the strong-typed message
			var reqMsg = (IRequestMessage)JsonSerializer.DeserializeFromString(data, msgType);
			return new RequestMessage
			{
				Name = name,
				Parameters = reqMsg.Parameters,
				Id = id,
			};
		}

		private ResponseMessage DeserializeResponse(string data, string name, string id, Error error)
		{
			// pre-deserialize to get the bulk of the message
			var type = MessageTypeProvider.GetResponseType(name);
			var msgType = typeof(ResponseMsg<>).MakeGenericType(new[] { type });

			// deserialize the strong-typed message
			var respMsg = (IResponseMessage)JsonSerializer.DeserializeFromString(data, msgType);
			return new ResponseMessage
			{
				Result = respMsg.Result,
				Error = error,
				Id = id,
			};
		}
	}
}
