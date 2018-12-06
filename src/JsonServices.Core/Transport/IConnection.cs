using System.Security.Principal;

namespace JsonServices.Transport
{
	public interface IConnection
	{
		string ConnectionId { get; }

		IIdentity CurrentUser { get; set; }
	}
}