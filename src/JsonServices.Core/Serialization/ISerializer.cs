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
		string Serialize(IMessage message);

		IMessage Deserialize(string data, IMessageTypeProvider typeProvider, IMessageNameProvider nameProvider);
	}
}
