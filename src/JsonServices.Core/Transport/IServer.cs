using System;
using System.Collections.Generic;

namespace JsonServices.Transport
{
	public interface IServer : IDisposable
	{
		void Send(string sessionId, string data);

		event EventHandler<MessageEventArgs> MessageReceived;

		IEnumerable<ISession> ActiveSessions { get; }

		ISession GetSession(string sessionId);
	}
}