using JsonServices.Services;

namespace JsonServices.Auth
{
	public class NullAuthProvider : IAuthProvider
	{
		public AuthResponse Authenticate(AuthRequest authRequest)
		{
			return new AuthResponse
			{
				// can't use WindowsIdentity.GetAnonymous() because it isn't portable
				AuthenticatedIdentity = new AnonymousIdentity(),
			};
		}
	}
}
