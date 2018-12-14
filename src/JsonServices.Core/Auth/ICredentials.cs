using System.Threading.Tasks;

namespace JsonServices.Auth
{
	public interface ICredentials
	{
		Task<string> Authenticate(JsonClient client);
	}
}
