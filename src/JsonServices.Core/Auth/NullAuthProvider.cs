using System.Security.Principal;

namespace JsonServices.Auth
{
	internal class NullAuthProvider : IAuthProvider
	{
		public AuthResponse Authenticate(AuthRequest authRequest)
		{
			return new AuthResponse
			{
				AuthenticatedIdentity = new JsonIdentity(string.Empty, "None"),
			};
		}
	}
}
