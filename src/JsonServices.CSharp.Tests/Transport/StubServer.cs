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
		public void Dispose() => Clients.Clear();

		private Dictionary<string, StubClient> Clients { get; } = new Dictionary<string, StubClient>();

		public IEnumerable<ISession> ActiveSessions => Clients.Values;

		public event EventHandler<MessageEventArgs> MessageReceived;

		public ISession GetSession(string sessionId) => Clients[sessionId];

		public void Send(string sessionId, string data)
		{
			var client = Clients[sessionId];
			client.Receive(data);
		}

		internal void Receive(string sessionId, string data)
		{
			var args = new MessageEventArgs
			{
				SessionId = sessionId,
				Data = data,
			};

			ThreadPool.QueueUserWorkItem(x => MessageReceived?.Invoke(this, args));
		}
	}
}
