using System;

namespace JsonServices.Transport
{
	public interface IClient : IDisposable
	{
		void Send(byte[] data);

		event EventHandler<MessageEventArgs> MessageReceived;
	}
}