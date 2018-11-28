using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices.Tests.Transport
{
	internal class StubServer : IServer
	{
		public void Start()
		{
		}

		public void Dispose() => Clients.Clear();

		private Dictionary<string, StubClient> Clients { get; } = new Dictionary<string, StubClient>();

		public IEnumerable<IConnection> Connections => Clients.Values;

		public event EventHandler<MessageEventArgs> MessageReceived;

		public IConnection GetConnection(string sessionId) => Clients[sessionId];

		public void Connect(StubClient client)
		{
			Clients[client.ConnectionId] = client;
		}

		public Task SendAsync(string sessionId, string data)
		{
			var client = Clients[sessionId];
			return client.ReceiveAsync(data);
		}

		internal Task ReceiveAsync(string sessionId, string data)
		{
			var args = new MessageEventArgs
			{
				ConnectionId = sessionId,
				Data = data,
			};

			return Task.Run(() => MessageReceived?.Invoke(this, args));
		}
	}
}
