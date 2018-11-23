using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonServer : IDisposable
	{
		public JsonServer(IServer server)
		{
			Server = server ?? throw new ArgumentNullException("server");
			Server.MessageReceived += HandleServerMessage;
		}

		public bool IsDisposed { get; private set; }

		public IServer Server { get; }

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
		}
	}
}
