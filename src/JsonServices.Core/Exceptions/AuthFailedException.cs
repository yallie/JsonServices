using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Exceptions
{
	public class AuthFailedException : JsonServicesException
	{
		public AuthFailedException()
			: base(1, "e")
		{
		}
	}
}
