using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Services;

namespace JsonServices.Serialization
{
	public interface ISerializer
	{
		IMessageTypeProvider MessageTypeProvider { get; }

		IMessageNameProvider MessageNameProvider { get; set; }

		string Serialize(IMessage message);

		IMessage Deserialize(string data);
	}
}
