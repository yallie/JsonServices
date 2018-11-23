using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;

namespace JsonServices.Serialization
{
	public interface ISerializer
	{
		byte[] SerializeRequest(RequestMessage message);

		byte[] SerializeResponse(ResponseMessage message);

		RequestMessage DeserializeRequest(byte[] data);

		ResponseMessage DeserializeResponse(string name, byte[] data);
	}
}
