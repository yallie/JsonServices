using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class InternalErrorException : JsonServicesException
	{
		public InternalErrorException(string message)
			: base(-32603, $"Internal error: {message}")
		{
		}
	}
}
