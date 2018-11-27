using System;
using System.Collections.Generic;

namespace JsonServices.Transport
{
	public interface IServer : IDisposable
	{
		void Start();

		void Send(string connectionId, string data);

		event EventHandler<MessageEventArgs> MessageReceived;

		event EventHandler<MessageFailureEventArgs> MessageSendFailure;

		IEnumerable<IConnection> Connections { get; }

		IConnection GetConnection(string connectionId);
	}
}