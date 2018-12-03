using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using FleckWebSocketServer = Fleck.WebSocketServer;

namespace JsonServices.Transport.Fleck
{
	public class FleckServer : IServer
	{
		public FleckServer(string url)
		{
			WebSocketServer = new FleckWebSocketServer(Url = url);
		}

		public string Url { get; }

		private FleckWebSocketServer WebSocketServer { get; set; }

		public IEnumerable<IConnection> Connections => FleckSessions.Values.ToArray();

		private ConcurrentDictionary<string, FleckSession> FleckSessions { get; } =
			new ConcurrentDictionary<string, FleckSession>();

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageEventArgs> ClientConnected;

		public event EventHandler<MessageEventArgs> ClientDisconnected;

		public void Start()
		{
			WebSocketServer.Start(socket =>
			{
				var session = new FleckSession(socket);

				socket.OnOpen = () =>
				{
					FleckSessions[session.ConnectionId] = session;
					ClientConnected?.Invoke(this, new MessageEventArgs
					{
						ConnectionId = session.ConnectionId,
					});
				};

				socket.OnClose = () =>
				{
					FleckSessions.TryRemove(session.ConnectionId, out var ignored);
					ClientDisconnected?.Invoke(this, new MessageEventArgs
					{
						ConnectionId = session.ConnectionId,
					});
				};

				socket.OnMessage = message =>
				{
					// looks like OnMessage can come earlier than OnOpen
					FleckSessions[session.ConnectionId] = session;
					MessageReceived?.Invoke(this, new MessageEventArgs
					{
						ConnectionId = session.ConnectionId,
						Data = message,
					});
				};
			});
		}

		public void Dispose()
		{
			if (WebSocketServer != null)
			{
				WebSocketServer.Dispose();
				WebSocketServer = null;
			}
		}

		public IConnection TryGetConnection(string sessionId) =>
			FleckSessions.TryGetValue(sessionId, out var result) ? result : null;

		public async Task SendAsync(string sessionId, string data)
		{
			if (FleckSessions.TryGetValue(sessionId, out var session))
			{
				await session.Socket.Send(data);
				return;
			}

			throw new InternalErrorException($"Session not found: {sessionId}. Known sessions: {string.Join(", ", FleckSessions.Keys)}");
		}
	}
}
