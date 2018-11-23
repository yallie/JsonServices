using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	public interface IResponseMessage
	{
		string Id { get; }

		object Result { get; }

		Error Error { get; }
	}
}
