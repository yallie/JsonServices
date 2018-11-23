using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices
{
	public class JsonClient : IDisposable
	{
		public JsonClient(IClient client)
		{
			Client = client ?? throw new ArgumentNullException("client");
			Client.MessageReceived += HandleClientMessage;
		}

		public bool IsDisposed { get; private set; }

		public IClient Client { get; set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Client.MessageReceived -= HandleClientMessage;
				Client.Dispose();
				IsDisposed = true;
			}
		}

		private void HandleClientMessage(object sender, MessageEventArgs args)
		{
		}
	}
}
