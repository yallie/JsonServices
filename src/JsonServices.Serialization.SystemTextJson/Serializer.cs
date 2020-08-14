using System;
using System.Text.Json;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization.SystemTextJson.Internal;
using JsonServices.Services;

namespace JsonServices.Serialization.SystemTextJson
{
	public class Serializer : ISerializer
	{
		public string Serialize(IMessage message) => JsonSerializer.Serialize(message);

		public IMessage Deserialize(string data, IMessageTypeProvider typeProvider, IMessageNameProvider nameProvider)
		{
			var preview = default(GenericMessage);
			try
			{
				preview = JsonSerializer.Deserialize<GenericMessage>(data);
			}
			catch
			{
				// invalid message format
			}

			if (preview == null || !preview.IsValid)
			{
				throw new InvalidRequestException(data)
				{
					MessageId = preview?.Id,
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
					throw new InvalidRequestException(name)
					{
						MessageId = preview.Id,
					};
				}
			}

			try
			{
				// deserialize request or response message
				if (isRequest)
				{
					return DeserializeRequest(data, name, preview.Id, typeProvider);
				}

				return DeserializeResponse(data, name, preview.Id, preview.Error, typeProvider);
			}
			catch (JsonServicesException ex)
			{
				// make sure MessageId is reported
				if (ex.MessageId == null)
				{
					ex.MessageId = preview.Id;
				}

				throw;
			}
			catch (Exception ex)
			{
				throw new InvalidRequestException(data, ex)
				{
					MessageId = preview.Id,
				};
			}
		}

		private RequestMessage DeserializeRequest(string data, string name, string id, IMessageTypeProvider typeProvider)
		{
			// get the message request type
			var type = typeProvider.GetRequestType(name);
			var msgType = typeof(RequestMsg<>).MakeGenericType(new[] { type });

			// deserialize the strong-typed message
			var reqMsg = (IRequestMessage)JsonSerializer.Deserialize(data, msgType);
			return new RequestMessage
			{
				Name = name,
				Parameters = reqMsg.Parameters,
				Id = id,
			};
		}

		public ResponseMessage DeserializeResponse(string data, string name, string id, Error error, IMessageTypeProvider typeProvider)
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
			var respMsg = (IResponseMessage)JsonSerializer.Deserialize(data, msgType);
			return ResponseMessage.Create(respMsg.Result, error, id);
		}
	}
}
