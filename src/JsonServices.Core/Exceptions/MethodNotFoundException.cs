using System;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class MethodNotFoundException : JsonServicesException
	{
		public const int ErrorCode = -32601;

		public MethodNotFoundException(string name)
			: base(ErrorCode, $"Method not found: {name}")
		{
		}

		public MethodNotFoundException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}

		internal MethodNotFoundException()
			: this("test")
		{
			// for unit tests
		}
	}
}
