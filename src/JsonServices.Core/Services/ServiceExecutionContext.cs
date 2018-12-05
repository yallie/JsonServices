using JsonServices.Transport;

namespace JsonServices.Services
{
	public class ServiceExecutionContext
	{
		public JsonServer Server { get; set; }

		public string ConnectionId { get; set; }

		public IConnection Connection => Server.Server.TryGetConnection(ConnectionId);
	}
}
