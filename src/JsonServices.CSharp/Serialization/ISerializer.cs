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
		string SerializeRequest(RequestMessage message);

		string SerializeResponse(ResponseMessage message);

		RequestMessage DeserializeRequest(string data);

		ResponseMessage DeserializeResponse(string data, Func<string, string> getName);
	}
}
