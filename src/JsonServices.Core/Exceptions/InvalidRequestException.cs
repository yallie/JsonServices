using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class InvalidRequestException : JsonServicesException
	{
		public InvalidRequestException(string data)
			: base(-32600, $"Invalid request. Request data: {data}")
		{
		}
	}
}
