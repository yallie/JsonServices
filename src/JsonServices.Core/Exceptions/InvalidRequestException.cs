using System;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class InvalidRequestException : JsonServicesException
	{
		public const int ErrorCode = -32600;

		public InvalidRequestException(string data)
			: base(ErrorCode, $"Invalid request. Request data: {data}")
		{
		}

		internal InvalidRequestException()
			: this("test")
		{
			// for unit tests
		}
	}
}
