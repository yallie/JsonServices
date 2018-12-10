using System;

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

		internal ParseErrorException()
			: this("test")
		{
		}
	}
}
