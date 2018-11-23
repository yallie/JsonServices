using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		public byte[] Serialize(IRequestMessage message)
		{
			using (var ms = new MemoryStream())
			{
				var tmp = new Dictionary<string, object>
				{
					{ "jsonrpc", "2.0" },
					{ "method", message.Name },
					{ "params", message.Params },
				};

				if (!string.IsNullOrWhiteSpace(message.Id))
				{
					tmp["id"] = message.Id;
				}

				using (var config = JsConfig.BeginScope())
				{
					config.IncludeTypeInfo = false;
					config.ExcludeTypeInfo = true;
					JsonSerializer.SerializeToStream(tmp, ms);
				}

				return ms.ToArray();
			}
		}

		public byte[] Serialize(IResponseMessage message)
		{
			using (var ms = new MemoryStream())
			{
				var tmp = new Dictionary<string, object>
				{
					{ "jsonrpc", "2.0" },
				};

				if (message.Error != null)
				{
					tmp["error"] = message.Error;
				}
				else
				{
					tmp["result"] = message.Result;
				}

				if (!string.IsNullOrWhiteSpace(message.Id))
				{
					tmp["id"] = message.Id;
				}

				using (var config = JsConfig.BeginScope())
				{
					config.IncludeTypeInfo = false;
					config.ExcludeTypeInfo = true;
					JsonSerializer.SerializeToStream(tmp, ms);
				}

				return ms.ToArray();
			}
		}

		public IRequestMessage DeserializeRequest(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			{
				// pre-deserialize to get the message request type
				var msg = JsonSerializer.DeserializeFromStream<RequestMessage<object>>(ms);
				var type = Locator.GetRequestType(msg.Name);

				// rewind the stream and deserialize the strong-typed message
				ms.Position = 0;
				return (IRequestMessage)JsonSerializer.DeserializeFromStream(type, ms);
			}
		}

		public IResponseMessage DeserializeResponse(string name, byte[] data)
		{
			using (var ms = new MemoryStream(data))
			{
				var type = Locator.GetResponseType(name);
				return (IResponseMessage)JsonSerializer.DeserializeFromStream(type, ms);
			}
		}
	}
}
