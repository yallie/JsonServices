using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Transport;
using WebSocketSharp;
using MessageEventArgs = JsonServices.Transport.MessageEventArgs;
using WsMessageEventArgs = WebSocketSharp.MessageEventArgs;

namespace JsonServices.WebSocketSharp
{
	public class WebSocketClient : IClient
	{
		public WebSocketClient(string url)
		{
			// add service name to the root url
			WebSocket = new WebSocket(url.TrimEnd('\\', '/').Trim() + WebSocketSession.ServiceName);
			WebSocket.OnMessage += OnMessageReceived;
		}

		public void Connect()
		{
			WebSocket.Connect();
		}

		private WebSocket WebSocket { get; set; }

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageFailureEventArgs> MessageSendFailure;

		private void OnMessageReceived(object sender, WsMessageEventArgs e)
		{
			// looks like WebSocket.OneMessage is executed under a lock,
			// so use the worker thread to avoid deadlock
			ThreadPool.QueueUserWorkItem(x =>
			{
				MessageReceived?.Invoke(this, new MessageEventArgs
				{
					Data = e.Data
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

		public void Send(string data)
		{
			#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			SendAsync(data);
			#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		private async Task SendAsync(string data)
		{
			try
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
			catch (Exception ex)
			{
				MessageSendFailure?.Invoke(this, new MessageFailureEventArgs
				{
					////SessionId = sessionId,
					Data = data,
					Exception = ex,
				});
			}
		}
	}
}
