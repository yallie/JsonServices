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
		private JsonSerializer JsonSerializer { get; set; } = JsonSerializer.Create();

		public string Serialize(IMessage message)
		{
			using (var sw = new StringWriter())
			{
				JsonSerializer.Serialize(sw, message);
				return sw.ToString();
			}
		}

		public IMessage Deserialize(string data, IMessageTypeProvider typeProvider, IMessageNameProvider nameProvider)
		{
			using (var sr = new StringReader(data ?? string.Empty))
			{
				// validate message, detect message type and name
				var preview = default(GenericMessage);
				try
				{
					preview = (GenericMessage)JsonSerializer.Deserialize(sr, typeof(GenericMessage));
				}
				catch (JsonReaderException)
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
			}
		}

		private RequestMessage DeserializeRequest(string data, string name, string id, IMessageTypeProvider typeProvider)
		{
			using (var sr = new StringReader(data))
			{
				// get the message request type
				var type = typeProvider.GetRequestType(name);
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

		public ResponseMessage DeserializeResponse(string data, string name, string id, Error error, IMessageTypeProvider typeProvider)
		{
			using (var sr = new StringReader(data))
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
				var respMsg = (IResponseMessage)JsonSerializer.Deserialize(sr, msgType);
				return ResponseMessage.Create(respMsg.Result, error, id);
			}
		}
	}
}
