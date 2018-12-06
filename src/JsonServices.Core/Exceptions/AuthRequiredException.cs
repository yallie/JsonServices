using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class AuthRequiredException : JsonServicesException
	{
		public AuthRequiredException(string name = null)
			: base(-32002, "Authentication is required" +
				  (!string.IsNullOrWhiteSpace(name) ? $": {name}" : string.Empty))
		{
		}
	}
}
