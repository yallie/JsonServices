﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization.SystemTextJson.Internal;
using JsonServices.Services;

namespace JsonServices.Serialization.SystemTextJson
{
	public class Serializer : ISerializer
	{
		private JsonSerializerOptions DefaultOptions { get; } = CreateOptions();

		private static JsonSerializerOptions CreateOptions()
		{
			var options = new JsonSerializerOptions();
#if NET_461
			options.IgnoreNullValues = false;
#else
			options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
#endif
			options.Converters.Add(new AnonymousConverterFactory());
			options.Converters.Add(new CultureInfoConverter());
			options.Converters.Add(new ObjectConverter());
			options.Converters.Add(new TupleConverterFactory());
			return options;
		}

		public string Serialize(IMessage message)
		{
			// System.Text.Json doesn't support DataMember attributes,
			// so we have to transform the objects to rename their properties
			switch (message)
			{
				case RequestMessage m:
					return Serialize(m);

				case ResponseErrorMessage m:
					return Serialize(m);

				case ResponseResultMessage m:
					return Serialize(m);

				default:
					throw new NotSupportedException();
			}
		}

		private string Serialize(RequestMessage rm)
		{
			// there is currently no way to skip the id property if it's null
			return rm.Id == null ? JsonSerializer.Serialize(new
			{
				jsonrpc = rm.Version,
				method = rm.Name,
				@params = rm.Parameters,
			},
			DefaultOptions) : JsonSerializer.Serialize(new
			{
				jsonrpc = rm.Version,
				method = rm.Name,
				@params = rm.Parameters,
				id = rm.Id,
			},
			DefaultOptions);
		}

		private string Serialize(ResponseErrorMessage rm)
		{
			return rm.Id == null ? JsonSerializer.Serialize(new
			{
				jsonrpc = rm.Version,
				error = rm.Error == null ? null : new
				{
					code = rm.Error.Code,
					message = rm.Error.Message,
					data = rm.Error.Data,
				},
			},
			DefaultOptions) : JsonSerializer.Serialize(new
			{
				jsonrpc = rm.Version,
				error = rm.Error == null ? null : new
				{
					code = rm.Error.Code,
					message = rm.Error.Message,
					data = rm.Error.Data,
				},
				id = rm.Id,
			},
			DefaultOptions);
		}

		private string Serialize(ResponseResultMessage rm)
		{
			return rm.Id == null ? JsonSerializer.Serialize(new
			{
				jsonrpc = rm.Version,
				result = rm.Result,
			},
			DefaultOptions) : JsonSerializer.Serialize(new
			{
				jsonrpc = rm.Version,
				result = rm.Result,
				id = rm.Id,
			},
			DefaultOptions);
		}

		public IMessage Deserialize(string data, IMessageTypeProvider typeProvider, IMessageNameProvider nameProvider)
		{
			var preview = default(GenericMessage);
			try
			{
				preview = JsonSerializer.Deserialize<GenericMessage>(data, DefaultOptions);
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
			var reqMsg = (IRequestMessage)JsonSerializer.Deserialize(data, msgType, DefaultOptions);
			return new RequestMessage
			{
				Name = name,
				Parameters = reqMsg.Parameters,
				Id = id,
			};
		}

		public ResponseMessage DeserializeResponse(string data, string name, string id, GenericError error, IMessageTypeProvider typeProvider)
		{
			// pre-deserialize to get the bulk of the message
			var type = typeProvider.GetResponseType(name);
			var err = error == null ? null : new Error
			{
				Code = error.Code,
				Data = error.Data,
				Message = error.Message,
			};

			// handle void messages
			if (type == typeof(void))
			{
				return ResponseMessage.Create(null, err, id);
			}

			// deserialize the strong-typed message
			var msgType = typeof(ResponseMsg<>).MakeGenericType(new[] { type });
			var respMsg = (IResponseMessage)JsonSerializer.Deserialize(data, msgType, DefaultOptions);
			return ResponseMessage.Create(respMsg.Result, err, id);
		}
	}
}
