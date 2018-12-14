using System.Security.Principal;

namespace JsonServices.Auth
{
	public class NullAuthProvider : IAuthProvider
	{
		public AuthResponse Authenticate(AuthRequest authRequest)
		{
			return new AuthResponse
			{
				AuthenticatedIdentity = new GenericIdentity(string.Empty, "None"),
			};
		}
	}
}
