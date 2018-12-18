using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace JsonServices.Transport.NetMQ
{
	public class NetMQClient : IClient
	{
		public NetMQClient(string url)
		{
			var id = Guid.NewGuid();
			ConnectionId = id.ToString();
			DealerSocket = new DealerSocket(url);
			DealerSocket.Options.Identity = id.ToByteArray();
			DealerSocket.ReceiveReady += DealerSocket_ReceiveReady;
			SendQueue = new NetMQQueue<byte[]>();
			SendQueue.ReceiveReady += SendQueue_ReceiveReady;
			SocketPoller = new NetMQPoller { DealerSocket, SendQueue };
			SocketPoller.RunAsync();
			ReceivedQueue = new NetMQQueue<MessageEventArgs>();
			ReceivedQueue.ReceiveReady += ReceivedQueue_ReceiveReady;
			ClientPoller = new NetMQPoller { ReceivedQueue };
			ClientPoller.RunAsync();
		}

		public Task ConnectAsync()
		{
			Connected?.Invoke(this, EventArgs.Empty);
			return Task.FromResult(true);
		}

		public Task DisconnectAsync()
		{
			Disconnected?.Invoke(this, EventArgs.Empty);
			return Task.FromResult(true);
		}

		private string ConnectionId { get; }

		private DealerSocket DealerSocket { get; }

		private NetMQQueue<byte[]> SendQueue { get; }

		private NetMQQueue<MessageEventArgs> ReceivedQueue { get; }

		private NetMQPoller SocketPoller { get; }

		private NetMQPoller ClientPoller { get; }

		public event EventHandler<MessageEventArgs> MessageReceived;

		public event EventHandler Connected;

		public event EventHandler Disconnected;

		public Task SendAsync(string data)
		{
			// enqueue instead of sending
			var bytes = Encoding.UTF8.GetBytes(data);
			SendQueue.Enqueue(bytes);
			return Task.FromResult(true);
		}

		private TimeSpan DequeueTimeout { get; } = TimeSpan.FromSeconds(1);

		private void SendQueue_ReceiveReady(object sender, NetMQQueueEventArgs<byte[]> args)
		{
			// executes on same poller thread as dealer socket, so we can send directly
			var message = new NetMQMessage();
			message.AppendEmptyFrame();

			if (args.Queue.TryDequeue(out var bytes, DequeueTimeout))
			{
				// add single frame
				message.Append(bytes);
				DealerSocket.SendMultipartMessage(message);
			}
		}

		private void DealerSocket_ReceiveReady(object sender, NetMQSocketEventArgs args)
		{
			// executes on same poller thread as dealer socket,
			// so we should enqueue messages to the received queue
			var msg = args.Socket.ReceiveMultipartMessage();
			if (msg == null || msg.IsEmpty)
			{
				return;
			}

			// we can only have one data frame
			var data = msg.First(f => !f.IsEmpty).Buffer;
			var message = new MessageEventArgs
			{
				ConnectionId = ConnectionId,
				Data = Encoding.UTF8.GetString(data),
			};

			ReceivedQueue.Enqueue(message);
		}

		private void ReceivedQueue_ReceiveReady(object sender, NetMQQueueEventArgs<MessageEventArgs> args)
		{
			// executes on client poller thread to avoid tying up the dealer socket poller thread
			if (args.Queue.TryDequeue(out var message, DequeueTimeout))
			{
				MessageReceived?.Invoke(this, new MessageEventArgs
				{
					ConnectionId = ConnectionId,
					Data = message.Data,
				});
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
				DealerSocket.Dispose();
				ClientPoller.Dispose();
				ReceivedQueue.Dispose();
			}
		}
	}
}
