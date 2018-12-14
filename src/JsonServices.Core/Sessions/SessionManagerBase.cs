using System;
using System.Collections.Concurrent;
using System.Security.Principal;

namespace JsonServices.Sessions
{
	public class SessionManagerBase : ISessionManager
	{
		private ConcurrentDictionary<string, Session> Sessions { get; } =
			new ConcurrentDictionary<string, Session>();

		public Session CreateSession(IIdentity currentUser)
		{
			var session = new Session
			{
				SessionId = Guid.NewGuid().ToString(),
				CurrentUser = currentUser,
			};

			Sessions[session.SessionId] = session;
			return session;
		}

		public Session TryGetSession(string sessionId)
		{
			return sessionId != null && Sessions.TryGetValue(sessionId, out var result) ? result : null;
		}

		public void DeleteSession(string sessionId)
		{
			Sessions.TryRemove(sessionId, out var ignored);
		}

		public void RenewSession(string sessionId)
		{
			var session = TryGetSession(sessionId);
			if (session != null)
			{
				session.Timestamp = DateTimeOffset.Now;
			}
		}
	}
}
