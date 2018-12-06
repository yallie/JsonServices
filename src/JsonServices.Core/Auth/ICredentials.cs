using System.Threading.Tasks;

namespace JsonServices.Auth
{
	public interface ICredentials
	{
		Task Authenticate(JsonClient client);
	}
}
