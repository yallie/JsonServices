using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JsonServices.Transport;
using WsSharpServer = WebSocketSharp.Server.WebSocketServer;

namespace JsonServices.WebSocketSharp
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

		public event EventHandler<MessageFailureEventArgs> MessageSendFailure;

		public void Start()
		{
			WsSharpServer.AddWebSocketService(WebSocketSession.ServiceName, () =>
			{
				var s = new WebSocketSession();

				s.OnOpenHandler = () =>
				{
					var sessionId = s.ID.ToString();
					WebSocketSessions[sessionId] = s;
				};

				s.OnCloseHandler = e =>
				{
					var sessionId = s.ID.ToString();
					WebSocketSessions.TryRemove(sessionId, out var ignored);
				};

				s.OnMessageHandler = message =>
				{
					ThreadPool.QueueUserWorkItem(x =>
					{
						var sessionId = s.ID.ToString();
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

		public IConnection GetConnection(string sessionId) => WebSocketSessions[sessionId];

		public void Send(string sessionId, string data)
		{
			#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			SendAsync(sessionId, data);
			#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		private async Task SendAsync(string sessionId, string data)
		{
			try
			{
				var session = WebSocketSessions[sessionId];
				var tcs = new TaskCompletionSource<bool>();
				session.Context.WebSocket.SendAsync(data, result =>
				{
					if (result)
					{
						tcs.TrySetResult(result);
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
					ConnectionId = sessionId,
					Data = data,
					Exception = ex,
				});
			}
		}
	}
}
