using System;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class AuthFailedException : JsonServicesException
	{
		public const int ErrorCode = -32001;

		public AuthFailedException(string message = null)
			: base(ErrorCode, message ?? "Authentication failed")
		{
		}

		internal AuthFailedException()
			: this(default(string))
		{
			// for unit tests
		}

		public AuthFailedException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}
	}
}
