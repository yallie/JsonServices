using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class MethodNotFoundException : JsonServicesException
	{
		public MethodNotFoundException(string name)
			: base(-32601, $"Method not found: {name}")
		{
		}
	}
}
