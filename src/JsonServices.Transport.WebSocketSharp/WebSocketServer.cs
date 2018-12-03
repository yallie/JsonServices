using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using WsSharpServer = WebSocketSharp.Server.WebSocketServer;

namespace JsonServices.Transport.WebSocketSharp
{
	public class WebSocketServer : IServer
	{
		public WebSocketServer(string url)
		{
			WsSharpServer = new WsSharpServer(Url = url);
		}

		public string Url { get; }

		private WsSharpServer WsSharpServer { get; set; }

		public IEnumerable<IConnection> Connections => WebSocketSessions.Values.ToArray();

		private ConcurrentDictionary<string, WebSocketSession> WebSocketSessions { get; } =
			new ConcurrentDictionary<string, WebSocketSession>();

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageEventArgs> ClientConnected;

		public event EventHandler<MessageEventArgs> ClientDisconnected;

		public void Start()
		{
			WsSharpServer.AddWebSocketService(WebSocketSession.ServiceName, () =>
			{
				var s = new WebSocketSession();

				s.OnOpenHandler = () =>
				{
					var sessionId = s.ID.ToString();
					WebSocketSessions[sessionId] = s;
					ClientConnected?.Invoke(this, new MessageEventArgs
					{
						ConnectionId = sessionId,
					});
				};

				s.OnCloseHandler = e =>
				{
					var sessionId = s.ID.ToString();
					WebSocketSessions.TryRemove(sessionId, out var ignored);
					ClientDisconnected?.Invoke(this, new MessageEventArgs
					{
						ConnectionId = sessionId,
					});
				};

				s.OnMessageHandler = message =>
				{
					// OnMessage can come earlier than OnOpen
					var sessionId = s.ID.ToString();
					WebSocketSessions[sessionId] = s;

					// looks like OnMessageHandler is called inside a lock statement,
					// so I'm using a thread pool to handle the message
					ThreadPool.QueueUserWorkItem(x =>
					{
						MessageReceived?.Invoke(this, new MessageEventArgs
						{
							ConnectionId = sessionId,
							Data = message.Data,
						});
					});
				};

				return s;
			});

			WsSharpServer.Start();
		}

		public void Dispose()
		{
			if (WsSharpServer != null)
			{
				WsSharpServer.Stop();
				WsSharpServer = null;
			}
		}

		public IConnection TryGetConnection(string sessionId) =>
			WebSocketSessions.TryGetValue(sessionId, out var result) ? result : null;

		public async Task SendAsync(string sessionId, string data)
		{
			if (!WebSocketSessions.TryGetValue(sessionId, out var session))
			{
				throw new InternalErrorException($"Session not found: {sessionId}. Known sessions: {string.Join(", ", WebSocketSessions.Keys)}");
			}

			var tcs = new TaskCompletionSource<bool>();
			session.Context.WebSocket.SendAsync(data, result =>
			{
				if (result)
				{
					tcs.TrySetResult(result);
					return;
				}

				tcs.TrySetException(new InternalErrorException($"Error sending data to session {sessionId}."));
			});

			await tcs.Task.ConfigureAwait(false);
		}
	}
}
