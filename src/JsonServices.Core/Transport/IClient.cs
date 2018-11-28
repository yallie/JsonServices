using System;
using System.Threading.Tasks;

namespace JsonServices.Transport
{
	public interface IClient : IDisposable
	{
		Task ConnectAsync();

		Task SendAsync(string data);

		event EventHandler<MessageEventArgs> MessageReceived;
	}
}