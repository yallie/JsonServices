using System;

namespace JsonServices.Transport
{
	public interface IClient : IDisposable
	{
		void Send(string data);

		event EventHandler<MessageEventArgs> MessageReceived;
	}
}