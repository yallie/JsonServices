using JsonServices.Sessions;

namespace JsonServices.Transport
{
	public interface IConnection
	{
		string ConnectionId { get; }

		Session Session { get; set; }
	}
}