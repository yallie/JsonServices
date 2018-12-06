using System;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class AuthFailedException : JsonServicesException
	{
		public AuthFailedException()
			: base(-32001, "Authentication failed")
		{
		}
	}
}
