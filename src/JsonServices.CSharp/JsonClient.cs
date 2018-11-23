using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonClient : IDisposable
	{
		public JsonClient(IClient client, ISerializer serializer)
		{
			Client = client ?? throw new ArgumentNullException(nameof(client));
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Client.MessageReceived += HandleClientMessage;
		}

		public bool IsDisposed { get; private set; }

		private IClient Client { get; set; }

		private ISerializer Serializer { get; set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Client.MessageReceived -= HandleClientMessage;
				Client.Dispose();
				IsDisposed = true;
			}
		}

		private ConcurrentDictionary<string, TaskCompletionSource<object>> PendingMessages { get; } =
			new ConcurrentDictionary<string, TaskCompletionSource<object>>();

		internal void SendMessage(IRequestMessage requestMessage)
		{
			var data = Serializer.Serialize(requestMessage);
			if (!requestMessage.IsOneWay)
			{
				PendingMessages[requestMessage.Id] = new TaskCompletionSource<object>();
			}

			Client.Send(data);
		}

		private void HandleClientMessage(object sender, MessageEventArgs args)
		{
		}
	}
}
