using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices.Tests.Transport
{
	internal class StubServer : IServer
	{
		public void Start()
		{
		}

		public void Dispose()
		{
			foreach (var connectionId in Clients.Keys)
			{
				ClientDisconnected?.Invoke(this, new MessageEventArgs
				{
					ConnectionId = connectionId
				});
			}

			Clients.Clear();
		}

		private ConcurrentDictionary<string, StubClient> Clients { get; } =
			new ConcurrentDictionary<string, StubClient>();

		public IEnumerable<IConnection> Connections => Clients.Values;

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageEventArgs> ClientConnected;

		public event EventHandler<MessageEventArgs> ClientDisconnected;

		public IConnection TryGetConnection(string sessionId) =>
			Clients.TryGetValue(sessionId, out var result) ? result : null;

		public void Connect(StubClient client)
		{
			Clients[client.ConnectionId] = client;
			ClientConnected?.Invoke(this, new MessageEventArgs
			{
				ConnectionId = client.ConnectionId
			});
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
