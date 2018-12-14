using System;
using System.Security.Principal;

namespace JsonServices.Sessions
{
	public class Session
	{
		public string SessionId { get; internal set; }

		public DateTimeOffset CreationDate { get; internal set; } = DateTimeOffset.Now;

		public DateTimeOffset Timestamp { get; internal set; } = DateTimeOffset.Now;

		public IIdentity CurrentUser { get; internal set; }
	}
}
