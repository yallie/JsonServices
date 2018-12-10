using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JsonServices.Transport;

namespace JsonServices.Services
{
	public class RequestContext : IRequestContext
	{
		public JsonServer Server { get; set; }

		public string ConnectionId { get; set; }

		public IConnection Connection => Server.Server.TryGetConnection(ConnectionId);

		public IDictionary<string, object> Properties { get; } =
			new ConcurrentDictionary<string, object>();

		public virtual void Dispose()
		{
			var props = Properties;
			if (props != null)
			{
				var disposables = props.Values.OfType<IDisposable>().ToArray();
				foreach (var disposable in disposables)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
