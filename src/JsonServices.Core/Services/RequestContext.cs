using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JsonServices.Messages;
using JsonServices.Transport;

namespace JsonServices.Services
{
	public sealed class RequestContext : IDisposable
	{
		public JsonServer Server { get; internal set; }

		public string ConnectionId { get; internal set; }

		public IConnection Connection => Server.Server.TryGetConnection(ConnectionId);

		public RequestMessage RequestMessage { get; internal set; }

		public ResponseMessage ResponseMessage { get; internal set; }

		public IDictionary<string, object> Properties { get; } =
			new ConcurrentDictionary<string, object>();

		public static RequestContext Current => CurrentContextHolder.Value;

		internal static AsyncLocal<RequestContext> CurrentContextHolder { get; } =
			new AsyncLocal<RequestContext>();

		public void Dispose()
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
