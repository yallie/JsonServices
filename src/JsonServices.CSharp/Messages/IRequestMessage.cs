using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	public interface IRequestMessage
	{
		string Id { get; }

		bool IsOneWay { get; }

		string Name { get; }

		object Params { get; }
	}
}
