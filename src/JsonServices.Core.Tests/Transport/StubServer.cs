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

		public event EventHandler<MessageFailureEventArgs> MessageSendFailure;

		public IConnection GetConnection(string sessionId) => Clients[sessionId];

		public void Connect(StubClient client)
		{
			Clients[client.ConnectionId] = client;
		}

		public void Send(string sessionId, string data)
		{
			try
			{
				var client = Clients[sessionId];
				client.Receive(data);
			}
			catch (Exception ex)
			{
				MessageSendFailure?.Invoke(this, new MessageFailureEventArgs
				{
					ConnectionId = sessionId,
					Data = data,
					Exception = ex,
				});
			}
		}

		internal void Receive(string sessionId, string data)
		{
			var args = new MessageEventArgs
			{
				ConnectionId = sessionId,
				Data = data,
			};

			ThreadPool.QueueUserWorkItem(x => MessageReceived?.Invoke(this, args));
		}
	}
}
