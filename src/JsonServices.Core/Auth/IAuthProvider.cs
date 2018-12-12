using JsonServices.Services;

namespace JsonServices.Auth
{
	public interface IAuthProvider
	{
		AuthResponse Authenticate(AuthRequest authRequest);
	}
}
