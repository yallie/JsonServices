using System;
using System.Runtime.Serialization;
using JsonServices.Messages;

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

		public InternalErrorException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}

		protected InternalErrorException(SerializationInfo info, StreamingContext ctx)
			: base(info, ctx)
		{
		}

		internal InternalErrorException()
			: this("test")
		{
			// for unit tests
		}
	}
}
