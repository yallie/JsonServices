using System;
using System.Runtime.Serialization;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class ParseErrorException : JsonServicesException
	{
		public const int ErrorCode = -32700;

		public ParseErrorException(string data)
			: base(ErrorCode, $"Parse error. Message data: {data}")
		{
		}

		public ParseErrorException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}

		protected ParseErrorException(SerializationInfo info, StreamingContext ctx)
			: base(info, ctx)
		{
		}

		internal ParseErrorException()
			: this("test")
		{
			// for unit tests
		}
	}
}
