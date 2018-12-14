using System.Security.Principal;

namespace JsonServices.Sessions
{
	public interface ISessionManager
	{
		Session CreateSession(IIdentity currentUser);

		Session TryGetSession(string sessionId);

		void RenewSession(string sessionId);

		void DeleteSession(string sessionId);
	}
}
