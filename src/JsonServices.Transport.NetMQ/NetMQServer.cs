using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using NetMQ;
using NetMQ.Sockets;

namespace JsonServices.Transport.NetMQ
{
	public sealed class NetMQServer : IServer
	{
		public NetMQServer(string url)
		{
			SendQueue = new NetMQQueue<NetMQMessage>();
			SendQueue.ReceiveReady += SendQueue_ReceiveReady;
			RouterSocket = new RouterSocket(url);
			RouterSocket.Options.RouterMandatory = true;
			RouterSocket.ReceiveReady += RouterSocket_ReceiveReady;
			SocketPoller = new NetMQPoller { RouterSocket, SendQueue };
			SocketPoller.RunAsync();
			ReceivedQueue = new NetMQQueue<MessageEventArgs>();
			ReceivedQueue.ReceiveReady += ReceivedQueue_ReceiveReady;
			ServerPoller = new NetMQPoller { ReceivedQueue };
			ServerPoller.RunAsync();
		}

		public string Url { get; }

		private NetMQQueue<NetMQMessage> SendQueue { get; set; }

		private NetMQQueue<MessageEventArgs> ReceivedQueue { get; set; }

		private RouterSocket RouterSocket { get; set; }

		private NetMQPoller SocketPoller { get; set; }

		private NetMQPoller ServerPoller { get; set; }

		public IEnumerable<IConnection> Connections => NetMQSessions.Values.ToArray();

		private ConcurrentDictionary<string, NetMQSession> NetMQSessions { get; } =
			new ConcurrentDictionary<string, NetMQSession>();

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler<MessageEventArgs> ClientConnected;

		event EventHandler<MessageEventArgs> IServer.ClientDisconnected
		{
			add { } remove { } // event is not available for NetMQ transport
		}

		public void Start()
		{
		}

		private TimeSpan DequeueTimeout { get; } = TimeSpan.FromSeconds(1);

		// Occurs on socket polling thread to ensure sending and receiving on same thread
		private void SendQueue_ReceiveReady(object sender, NetMQQueueEventArgs<NetMQMessage> args)
		{
			if (args.Queue.TryDequeue(out var message, DequeueTimeout))
			{
				try
				{
					RouterSocket.SendMultipartMessage(message);
				}
				catch (Exception)
				{
					// TODO: report the exception —
					// connectionId not found, or network error, or whatever
				}
			}
		}

		// Occurs on socket polling thread to enssure sending and receiving on same thread
		private void RouterSocket_ReceiveReady(object sender, NetMQSocketEventArgs args)
		{
			// executes on same poller thread as router socket,
			// so we should enqueue messages to the received queue
			var msg = args.Socket.ReceiveMultipartMessage();
			if (msg == null || msg.FrameCount < 2)
			{
				// TODO: throw? invalid message format
				return;
			}

			// the first frame is ConnectionId
			if (msg[0].BufferSize != 16)
			{
				// TODO: throw? invalid client address
				return;
			}

			// we can only have one data frame
			var connectionId = new Guid(msg[0].Buffer).ToString();
			var data = msg.Skip(1).First(f => !f.IsEmpty).Buffer;
			var message = new MessageEventArgs
			{
				ConnectionId = connectionId,
				Data = Encoding.UTF8.GetString(data),
			};

			// register new client session if doesn't yet exist
			NetMQSessions.GetOrAdd(message.ConnectionId, id =>
			{
				var session = new NetMQSession { ConnectionId = id };
				ClientConnected?.Invoke(this, new MessageEventArgs { ConnectionId = id });
				return session;
			});

			// process the incoming message
			ReceivedQueue.Enqueue(message);
		}

		private void ReceivedQueue_ReceiveReady(object sender, NetMQQueueEventArgs<MessageEventArgs> args)
		{
			if (args.Queue.TryDequeue(out var message, DequeueTimeout))
			{
				MessageReceived?.Invoke(this, message);
			}
		}

		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				SocketPoller.Dispose();
				SendQueue.Dispose();
				RouterSocket.Dispose();
				ServerPoller.Dispose();
				ReceivedQueue.Dispose();
			}
		}

		public IConnection TryGetConnection(string sessionId) =>
			NetMQSessions.TryGetValue(sessionId, out var result) ? result : null;

		public Task SendAsync(string sessionId, string data)
		{
			if (!NetMQSessions.TryGetValue(sessionId, out var session))
			{
				throw new InternalErrorException($"Session not found: {sessionId}. Known sessions: {string.Join(", ", NetMQSessions.Keys)}");
			}

			var message = new NetMQMessage();
			var clientId = new Guid(sessionId).ToByteArray();
			var dataBytes = Encoding.UTF8.GetBytes(data);

			message.Append(clientId);
			message.AppendEmptyFrame();
			message.Append(dataBytes);

			SendQueue.Enqueue(message);
			return Task.FromResult(true); // Task.CompletedTask
		}
	}
}
