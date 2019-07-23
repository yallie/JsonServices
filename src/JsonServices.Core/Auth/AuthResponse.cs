using System.Collections.Generic;
using System.Security.Principal;

namespace JsonServices.Auth
{
	public class AuthResponse
	{
		public AuthResponse()
		{
		}

		public AuthResponse(JsonIdentity identity)
		{
			AuthenticatedIdentity = identity;
		}

		public Dictionary<string, string> Parameters { get; set; } =
			new Dictionary<string, string>();

		public string SessionId { get; set; } // optional

		public JsonIdentity AuthenticatedIdentity { get; set; }

		public bool Completed => AuthenticatedIdentity != null;
	}
}
