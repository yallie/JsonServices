using JsonServices.Services;

namespace JsonServices.Auth
{
	internal class AuthService
	{
		public AuthResponse Authenticate(AuthRequest authRequest)
		{
			var context = RequestContext.Current;
			var response = context.Server.AuthProvider.Authenticate(authRequest);
			if (response.Completed)
			{
				var sessionManager = context.Server.SessionManager;
				var session = sessionManager.TryGetSession(response.SessionId) ??
					sessionManager.CreateSession(response.SessionId, response.AuthenticatedIdentity);
				context.Connection.Session = session;
				response.SessionId = session.SessionId;
			}

			return response;
		}
	}
}
