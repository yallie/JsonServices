using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonServer : IDisposable
	{
		public JsonServer(IServer server, ISerializer serializer)
		{
			Server = server ?? throw new ArgumentNullException(nameof(server));
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			Server.MessageReceived += HandleServerMessage;
		}

		public bool IsDisposed { get; private set; }

		public IServer Server { get; }

		private ISerializer Serializer { get; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Server.MessageReceived -= HandleServerMessage;
				Server.Dispose();
				IsDisposed = true;
			}
		}

		private void HandleServerMessage(object sender, MessageEventArgs args)
		{
			var request = Serializer.DeserializeRequest(args.Data);
		}
	}
}
