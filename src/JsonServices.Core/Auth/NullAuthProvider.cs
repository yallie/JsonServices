using JsonServices.Services;

namespace JsonServices.Auth
{
	public class NullAuthProvider : IAuthProvider
	{
		public AuthResponse Authenticate(ServiceExecutionContext context, AuthRequest authRequest)
		{
			return new AuthResponse
			{
				AuthenticatedIdentity = new AnonymousIdentity(), // WindowsIdentity is not portable
			};
		}
	}
}
