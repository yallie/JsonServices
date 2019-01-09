using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JsonServices.Transport.Fleck
{
	public class FleckClient : IClient
	{
		public FleckClient(string url)
		{
			Url = new Uri(url);
			WebSocket = new ClientWebSocket();
		}

		public Uri Url { get; private set; }

		private ClientWebSocket WebSocket { get; set; }

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler Connected;

		public event EventHandler Disconnected;

		public async Task ConnectAsync()
		{
			await WebSocket.ConnectAsync(Url, CancellationToken.None);
			Connected?.Invoke(this, EventArgs.Empty);
			ListenTask = StartListening();
		}

		private Task ListenTask { get; set; }

		private async Task StartListening()
		{
			var buffer = new byte[1024 * 1024];
			var segment = new ArraySegment<byte>(buffer);

			try
			{
				while (WebSocket.State == WebSocketState.Open)
				{
					var sb = new StringBuilder();

					WebSocketReceiveResult result;
					do
					{
						result = await WebSocket.ReceiveAsync(segment,
							CancellationToken.None).ConfigureAwait(false);

						if (result.MessageType == WebSocketMessageType.Close)
						{
							await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
								string.Empty, CancellationToken.None).ConfigureAwait(false);
							Disconnected?.Invoke(this, EventArgs.Empty);
						}
						else
						{
							// TODO: it probably can crash if the server sends chunks not aligned
							// to UTF8 char bounaries (not sure if Fleck server can really do this)
							var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
							sb.Append(str);
						}
					}
					while (!result.EndOfMessage);

					MessageReceived?.Invoke(this, new MessageEventArgs
					{
						Data = sb.ToString(),
					});
				}
			}
			catch (Exception)
			{
				Disconnected?.Invoke(this, EventArgs.Empty);
			}
			finally
			{
				WebSocket.Dispose();
			}
		}

		public async Task DisconnectAsync()
		{
			await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Ok", CancellationToken.None);
			Disconnected?.Invoke(this, EventArgs.Empty);
		}

		public void Dispose()
		{
			WebSocket.Dispose();
		}

		public async Task SendAsync(string data)
		{
			var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
			await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
		}
	}
}
