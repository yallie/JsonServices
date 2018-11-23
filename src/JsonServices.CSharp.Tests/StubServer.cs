using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices.Tests
{
	internal class StubServer : IServer
	{
		public void Dispose() => Clients.Clear();

		private Dictionary<Guid, StubClient> Clients { get; } = new Dictionary<Guid, StubClient>();

		public IEnumerable<ISession> ActiveSessions => Clients.Values;

		public event EventHandler<MessageEventArgs> MessageReceived;

		public ISession GetSession(Guid sessionId) => Clients[sessionId];

		public void Send(Guid sessionId, byte[] data)
		{
			var client = Clients[sessionId];
			client.Receive(data);
		}

		internal void Receive(Guid sessionId, byte[] data)
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
