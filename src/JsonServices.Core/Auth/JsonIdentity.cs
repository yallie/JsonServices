using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Auth
{
	public class JsonIdentity : IIdentity
	{
		public JsonIdentity()
		{
		}

		public JsonIdentity(string name, string type)
		{
			Name = name;
			AuthenticationType = type;
		}

		public JsonIdentity(IIdentity identity)
			: this(identity?.Name, identity?.AuthenticationType)
		{
		}

		public string Name { get; set; }

		public string AuthenticationType { get; set; }

		public virtual bool IsAuthenticated => !string.IsNullOrEmpty(AuthenticationType);
	}
}
