using System;
using System.Collections.Generic;

namespace JsonServices.Transport
{
	public interface IServer : IDisposable
	{
		void Start();

		void Send(string sessionId, string data);

		event EventHandler<MessageEventArgs> MessageReceived;

		event EventHandler<MessageFailureEventArgs> MessageSendFailure;

		IEnumerable<ISession> ActiveSessions { get; }

		ISession GetSession(string sessionId);
	}
}