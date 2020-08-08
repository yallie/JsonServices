using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices.Sample.CoreServer
{
	public class JsonServicesConnectionManager : IServer
	{
		public bool Started { get; private set; }

		public void Start() =>  Started = true;

		public void Dispose() => Started = false;

		public IConnection TryGetConnection(string connectionId)
		{
			throw new NotImplementedException();
		}

		public Task SendAsync(string connectionId, string data)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<IConnection> Connections => throw new NotImplementedException();

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageEventArgs> ClientConnected;

		public event EventHandler<MessageEventArgs> ClientDisconnected;
	}
}
