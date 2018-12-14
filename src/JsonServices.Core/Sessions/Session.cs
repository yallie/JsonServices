using System;
using System.Security.Principal;

namespace JsonServices.Sessions
{
	public class Session
	{
		public Session(string sessionId, IIdentity currentUser, DateTimeOffset? creationDate = null)
		{
			SessionId = sessionId;
			CurrentUser = currentUser;
			CreationDate = creationDate ?? DateTimeOffset.Now;
		}

		public string SessionId { get; internal set; }

		public DateTimeOffset CreationDate { get; internal set; }

		public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

		public IIdentity CurrentUser { get; internal set; }
	}
}
