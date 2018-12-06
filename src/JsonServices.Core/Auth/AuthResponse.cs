using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Auth
{
	public class AuthResponse
	{
		public Dictionary<string, string> Parameters { get; set; }

		public bool Completed => AuthenticatedIdentity != null;

		public IIdentity AuthenticatedIdentity { get; set; }
	}
}
