using System;
using System.Security.Principal;
using System.Threading.Tasks;
using JsonServices.Sessions;
using JsonServices.Transport;

namespace JsonServices.Tests.Transport
{
	internal class StubClient : IClient, IConnection
	{
		public StubClient(StubServer server, string connectionId = null)
		{
			if (!string.IsNullOrWhiteSpace(connectionId))
			{
				ConnectionId = connectionId;
			}

			Server = server;
			Server.Connect(this);
		}

		public Task ConnectAsync()
		{
			Connected?.Invoke(this, EventArgs.Empty);
			return Task.FromResult(true); // Task.CompletedTask
		}

		public Task DisconnectAsync()
		{
			Disconnected?.Invoke(this, EventArgs.Empty);
			return Task.FromResult(true); // Task.CompletedTask
		}

		public void Dispose() => Server = null;

		public string ConnectionId { get; } = Guid.NewGuid().ToString();

		private StubServer Server { get; set; }

		public Session Session { get; set; }

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler Connected;

		public event EventHandler Disconnected;

		public Task SendAsync(string data)
		{
			return Server?.ReceiveAsync(ConnectionId, data);
		}

		internal Task ReceiveAsync(string data)
		{
			var args = new MessageEventArgs
			{
				ConnectionId = ConnectionId,
				Data = data,
			};

			return Task.Run(() => MessageReceived?.Invoke(this, args));
		}
	}
}
