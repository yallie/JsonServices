using System;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class AuthRequiredException : JsonServicesException
	{
		public const int ErrorCode = -32002;

		public AuthRequiredException(string name = null)
			: base(ErrorCode, "Authentication is required" +
				  (!string.IsNullOrWhiteSpace(name) ? $": {name}" : string.Empty))
		{
		}

		internal AuthRequiredException()
			: this("test")
		{
			// for unit tests
		}
	}
}
