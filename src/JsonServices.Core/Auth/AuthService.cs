using JsonServices.Services;

namespace JsonServices.Auth
{
	public class AuthService
	{
		public AuthResponse Authenticate(RequestContext context, AuthRequest authRequest)
		{
			var response = context.Server.AuthProvider.Authenticate(context, authRequest);
			if (response.Completed)
			{
				context.Connection.CurrentUser = response.AuthenticatedIdentity;
			}

			return response;
		}
	}
}
