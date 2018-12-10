using System;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class AuthFailedException : JsonServicesException
	{
		public const int ErrorCode = -32001;

		public AuthFailedException()
			: base(ErrorCode, "Authentication failed")
		{
		}
	}
}
