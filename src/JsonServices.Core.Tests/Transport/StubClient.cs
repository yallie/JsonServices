using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
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

		public Task ConnectAsync() => Task.FromResult(true); // Task.CompletedTask

		public void Dispose() => Server = null;

		public string ConnectionId { get; } = Guid.NewGuid().ToString();

		private StubServer Server { get; set; }

		public IIdentity CurrentUser { get; set; }

		public event EventHandler<MessageEventArgs> MessageReceived;

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
