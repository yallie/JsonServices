using System;
using System.Runtime.Serialization;
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

		public InvalidRequestException(string data, Exception innerException)
			: base(ErrorCode, $"Invalid request. Request data: {data}", innerException)
		{
		}

		public InvalidRequestException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}

		protected InvalidRequestException(SerializationInfo info, StreamingContext ctx)
			: base(info, ctx)
		{
		}

		internal InvalidRequestException()
			: this("test")
		{
			// for unit tests
		}
	}
}
