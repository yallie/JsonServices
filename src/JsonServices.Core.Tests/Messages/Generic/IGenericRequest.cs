using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages.Generic
{
	public interface IGenericRequest
	{
		string ValueType { get; }

		object Value { get; }

		object CreateResponse(object result);
	}
}
