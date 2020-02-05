using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WsMessageEventArgs = WebSocketSharp.MessageEventArgs;

namespace JsonServices.Transport.WebSocketSharp
{
	public class WebSocketClient : IClient
	{
		public WebSocketClient(string url)
		{
			// add service name to the root url
			WebSocket = new WebSocket(url.TrimEnd('\\', '/').Trim() + WebSocketSession.ServiceName);
			WebSocket.OnMessage += OnMessageReceived;
			WebSocket.OnClose += OnClose;
		}

		public async Task ConnectAsync()
		{
			await Task.Run(() => WebSocket.Connect());
			Connected?.Invoke(this, EventArgs.Empty);
		}

		public async Task DisconnectAsync()
		{
			await Task.Run(() => WebSocket.Close());
			Disconnected?.Invoke(this, EventArgs.Empty);
		}

		private WebSocket WebSocket { get; set; }

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler Connected;

		public event EventHandler Disconnected;

		private void OnClose(object sender, CloseEventArgs e)
		{
			Disconnected?.Invoke(this, e);
		}

		private void OnMessageReceived(object sender, WsMessageEventArgs e)
		{
			// looks like WebSocket.OneMessage is executed under a lock,
			// so use the worker thread to avoid deadlock
			ThreadPool.QueueUserWorkItem(x =>
			{
				MessageReceived?.Invoke(this, new MessageEventArgs
				{
					Data = e.Data,
				});
			});
		}

		public void Dispose()
		{
			if (WebSocket != null)
			{
				WebSocket.OnMessage -= OnMessageReceived;
				WebSocket.Close();
				WebSocket = null;
			}
		}

		public async Task SendAsync(string data)
		{
			var tcs = new TaskCompletionSource<bool>();
			WebSocket.SendAsync(data, result =>
			{
				if (result)
				{
					tcs.TrySetResult(true);
					return;
				}

				tcs.TrySetException(new Exception("Error sending data"));
			});

			await tcs.Task.ConfigureAwait(false);
		}
	}
}
