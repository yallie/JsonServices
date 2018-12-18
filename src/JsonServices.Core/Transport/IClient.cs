using System;
using System.Threading.Tasks;

namespace JsonServices.Transport
{
	public interface IClient : IDisposable
	{
		Task ConnectAsync();

		Task SendAsync(string data);

		Task DisconnectAsync();

		event EventHandler<MessageEventArgs> MessageReceived;

		event EventHandler Connected;

		event EventHandler Disconnected;
	}
}