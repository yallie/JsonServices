using System;
using System.Threading;
using JsonServices.Transport;

namespace JsonServices.Tests.Transport
{
	internal class StubClient : IClient, IConnection
	{
		public StubClient(StubServer server)
		{
			Server = server;
			Server.Connect(this);
		}

		public void Connect()
		{
		}

		public void Dispose() => Server = null;

		public string ConnectionId { get; } = Guid.NewGuid().ToString();

		private StubServer Server { get; set; }

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageFailureEventArgs> MessageSendFailure;

		public void Send(string data)
		{
			try
			{
				Server?.Receive(ConnectionId, data);
			}
			catch (Exception ex)
			{
				MessageSendFailure?.Invoke(this, new MessageFailureEventArgs
				{
					ConnectionId = ConnectionId,
					Data = data,
					Exception = ex,
				});
			}
		}

		internal void Receive(string data)
		{
			var args = new MessageEventArgs
			{
				ConnectionId = ConnectionId,
				Data = data,
			};

			ThreadPool.QueueUserWorkItem(x => MessageReceived?.Invoke(this, args));
		}
	}
}
