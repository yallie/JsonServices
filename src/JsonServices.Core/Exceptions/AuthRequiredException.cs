﻿using System;
using System.Runtime.Serialization;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	[Serializable]
	public class AuthRequiredException : JsonServicesException
	{
		public const int ErrorCode = -32002;

		public AuthRequiredException(string name = null)
			: base(ErrorCode, "Authentication is required" +
				  (!string.IsNullOrWhiteSpace(name) ? $": {name}" : string.Empty))
		{
		}

		public AuthRequiredException(Error error)
			: base(ErrorCode, error.Message)
		{
			Details = error.Data;
		}

		protected AuthRequiredException(SerializationInfo info, StreamingContext ctx)
			: base(info, ctx)
		{
		}

		internal AuthRequiredException()
			: this("test")
		{
			// for unit tests
		}
	}
}
