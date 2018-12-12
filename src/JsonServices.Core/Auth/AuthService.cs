using JsonServices.Services;

namespace JsonServices.Auth
{
	public class AuthService
	{
		public AuthResponse Authenticate(AuthRequest authRequest)
		{
			var context = RequestContext.Current;
			var response = context.Server.AuthProvider.Authenticate(authRequest);
			if (response.Completed)
			{
				context.Connection.CurrentUser = response.AuthenticatedIdentity;
			}

			return response;
		}
	}
}
