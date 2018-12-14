using JsonServices.Sessions;

namespace JsonServices.Transport.NetMQ
{
	public class NetMQSession : IConnection
	{
		public string ConnectionId { get; internal set; }

		public Session Session { get; set; }
	}
}
