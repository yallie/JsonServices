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
		byte[] Serialize(IRequestMessage message);

		byte[] Serialize(IResponseMessage message);

		IRequestMessage DeserializeRequest(byte[] data);

		IResponseMessage DeserializeResponse(string name, byte[] data);
	}
}
