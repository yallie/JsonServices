using System;
using JsonServices.Messages;

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

		public AuthFailedException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}
	}
}
