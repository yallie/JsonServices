using System;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class InternalErrorException : JsonServicesException
	{
		public const int ErrorCode = -32603;

		public InternalErrorException(string message)
			: base(ErrorCode, $"Internal error: {message}")
		{
		}

		internal InternalErrorException()
			: this("test")
		{
			// for unit tests
		}
	}
}
