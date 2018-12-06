using JsonServices.Services;

namespace JsonServices.Auth
{
	public interface IAuthProvider
	{
		AuthResponse Authenticate(ServiceExecutionContext context, AuthRequest authRequest);
	}
}
