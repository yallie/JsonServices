using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonServices.Transport
{
	public interface IServer : IDisposable
	{
		void Start();

		Task SendAsync(string connectionId, string data);

		event EventHandler<MessageEventArgs> MessageReceived;

		IEnumerable<IConnection> Connections { get; }

		IConnection GetConnection(string connectionId);
	}
}