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
			Client = client;
		}

		public bool IsDisposed { get; private set; }

		public IClient Client { get; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Client?.Dispose();
				IsDisposed = true;
			}
		}
	}
}
