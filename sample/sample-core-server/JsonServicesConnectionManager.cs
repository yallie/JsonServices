using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Sessions;
using JsonServices.Transport;

namespace JsonServices.Sample.CoreServer
{
	public class JsonServicesConnectionManager : IServer
	{
		public bool Started { get; private set; }

		public void Start() =>  Started = true;

		public void Dispose()
		{
			Started = false;

			// shutdown all running web sockets? hmm, can't do it synchronously
			foreach (var conn in WebSocketConnections)
			{
				var ws = conn.Value.WebSocket;
				if (ws.State == WebSocketState.Open)
				{
					// there is no ws.Close
				}
			}
		}

		public async Task HandleIncomingMessages(WebSocket webSocket)
		{
			// 1. create connection
			var connection = new WebSocketConnection(webSocket);
			WebSocketConnections.TryAdd(connection.ConnectionId, connection);
			ClientConnected?.Invoke(this, new MessageEventArgs
			{
				ConnectionId = connection.ConnectionId,
			});

			// 2. TODO: handle incoming messages in a loop until the socket is closed
			await ReceiveMessages(connection);

			// 3. disconnect the client
			ClientDisconnected?.Invoke(this, new MessageEventArgs
			{
				ConnectionId = connection.ConnectionId,
			});
		}

		private async Task ReceiveMessages(WebSocketConnection connection)
		{
			var webSocket = connection.WebSocket;
			var buffer = new byte[1024 * 32];
			var stringBuilder = new StringBuilder();

			while (webSocket.State == WebSocketState.Open)
			{
				var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Close)
				{
					return;
				}

				// if the message is too big to fit in the buffer, assemble it
				// TODO: message parts can be invalid UTF8 chars (broken into parts)
				var part = Encoding.UTF8.GetString(buffer);
				stringBuilder.Append(part);
				if (result.EndOfMessage)
				{
					MessageReceived?.Invoke(this, new MessageEventArgs
					{
						ConnectionId = connection.ConnectionId,
						Data = stringBuilder.ToString(),
					});

					stringBuilder.Clear();
				}
			}
		}

		private class WebSocketConnection : IConnection
		{
			public WebSocketConnection(WebSocket ws) => WebSocket = ws;

			public string ConnectionId { get; } = Guid.NewGuid().ToString();

			public WebSocket WebSocket { get; }

			public Session Session { get; set; }
		}

		private ConcurrentDictionary<string, WebSocketConnection> WebSocketConnections { get; } =
			new ConcurrentDictionary<string, WebSocketConnection>();

		public IConnection TryGetConnection(string connectionId) =>
			WebSocketConnections.TryGetValue(connectionId, out var value) ? value : null;

		public async Task SendAsync(string connectionId, string data)
		{
			if (WebSocketConnections.TryGetValue(connectionId, out var connection))
			{
				var buffer = Encoding.UTF8.GetBytes(data);
				await connection.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}

		public IEnumerable<IConnection> Connections => WebSocketConnections.Values;

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageEventArgs> ClientConnected;

		public event EventHandler<MessageEventArgs> ClientDisconnected;
	}
}
