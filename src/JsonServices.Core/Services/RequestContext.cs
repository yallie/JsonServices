using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JsonServices.Messages;
using JsonServices.Transport;

namespace JsonServices.Services
{
	public class RequestContext : IDisposable
	{
		public JsonServer Server { get; set; }

		public string ConnectionId { get; set; }

		public IConnection Connection => Server.Server.TryGetConnection(ConnectionId);

		public RequestMessage RequestMessage { get; set; }

		public IDictionary<string, object> Properties { get; } =
			new ConcurrentDictionary<string, object>();

		public static RequestContext Current => CurrentContextHolder.Value;

		internal static AsyncLocal<RequestContext> CurrentContextHolder { get; } =
			new AsyncLocal<RequestContext>();

		private static void ThreadContextChanged(AsyncLocalValueChangedArgs<RequestContext> args)
		{
			// reset current request context value for the new thread
			if (args.ThreadContextChanged && args.CurrentValue != null)
			{
				CurrentContextHolder.Value = null;
			}
		}

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
