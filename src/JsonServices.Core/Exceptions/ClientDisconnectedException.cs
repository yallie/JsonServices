using System;
using System.Runtime.Serialization;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class ClientDisconnectedException : JsonServicesException
	{
		public const int ErrorCode = -32003;

		public ClientDisconnectedException(string message = null)
			: base(ErrorCode, message ?? "Client is disconnected")
		{
		}

		internal ClientDisconnectedException()
			: this(default(string))
		{
			// for unit tests
		}

		public ClientDisconnectedException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}

		protected ClientDisconnectedException(SerializationInfo info, StreamingContext ctx)
			: base(info, ctx)
		{
		}
	}
}
