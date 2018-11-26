using System;

namespace JsonServices.Transport
{
	public interface IClient : IDisposable
	{
		void Connect();

		void Send(string data);

		event EventHandler<MessageEventArgs> MessageReceived;

		event EventHandler<MessageFailureEventArgs> MessageSendFailure;
	}
}