using System;
using JsonServices.Messages;

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

		public InvalidRequestException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}

		internal InvalidRequestException()
			: this("test")
		{
			// for unit tests
		}
	}
}
