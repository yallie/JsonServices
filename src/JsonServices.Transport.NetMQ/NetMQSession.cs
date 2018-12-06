using System.Security.Principal;

namespace JsonServices.Transport.NetMQ
{
	public class NetMQSession : IConnection
	{
		public string ConnectionId { get; internal set; }

		public IIdentity CurrentUser { get; set; }
	}
}
