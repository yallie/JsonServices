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
		private IDisposable ConfigureSerializer()
		{
			var config = JsConfig.BeginScope();
			config.IncludeNullValues = true;
			config.IncludeTypeInfo = false;
			config.ExcludeTypeInfo = true;
			config.DateHandler = DateHandler.ISO8601;
			config.AppendUtcOffset = true;
			return config;
		}

		public string Serialize(IMessage message)
		{
			using (ConfigureSerializer())
			{
				return JsonSerializer.SerializeToString(message);
			}
		}

		public IMessage Deserialize(string data, IMessageTypeProvider typeProvider, IMessageNameProvider nameProvider)
		{
			// validate message, detect message type and name
			var preview = (GenericMessage)JsonSerializer.DeserializeFromString(data, typeof(GenericMessage));
			if (!preview.IsValid)
			{
				throw new InvalidRequestException(data)
				{
					MessageId = preview.Id,
				};
			}

			// detect message name
			var name = preview.Name;
			var isRequest = name != null;
			if (name == null)
			{
				// server cannot handle a response message
				if (nameProvider == null)
				{
					throw new InvalidRequestException(data)
					{
						MessageId = preview.Id,
					};
				}

				// invalid request id
				name = nameProvider.TryGetMessageName(preview.Id);
				if (name == null)
				{
					throw new InvalidRequestException(data)
					{
						MessageId = preview.Id,
					};
				}
			}

			try
			{
				// deserialize request or response message
				using (ConfigureSerializer())
				{
					if (isRequest)
					{
						return DeserializeRequest(data, name, preview.Id, typeProvider);
					}

					return DeserializeResponse(data, name, preview.Id, preview.Error, typeProvider);
				}
			}
			catch (JsonServicesException ex)
			{
				if (ex.MessageId == null)
				{
					ex.MessageId = preview.Id;
				}

				throw;
			}
		}

		private RequestMessage DeserializeRequest(string data, string name, string id, IMessageTypeProvider typeProvider)
		{
			// pre-deserialize to get the message request type
			var type = typeProvider.GetRequestType(name);
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

		private ResponseMessage DeserializeResponse(string data, string name, string id, Error error, IMessageTypeProvider typeProvider)
		{
			// pre-deserialize to get the bulk of the message
			var type = typeProvider.GetResponseType(name);

			// handle void messages
			if (type == typeof(void))
			{
				return ResponseMessage.Create(null, error, id);
			}

			// deserialize the strong-typed message
			var msgType = typeof(ResponseMsg<>).MakeGenericType(new[] { type });
			var respMsg = (IResponseMessage)JsonSerializer.DeserializeFromString(data, msgType);
			return ResponseMessage.Create(respMsg.Result, error, id);
		}
	}
}
