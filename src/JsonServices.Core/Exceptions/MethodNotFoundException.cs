using System;

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

		internal MethodNotFoundException()
			: this("test")
		{
			// for unit tests
		}
	}
}
