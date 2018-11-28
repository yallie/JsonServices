using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;

namespace JsonServices.Services
{
	public class ServiceExecutor : IServiceExecutor
	{
		private ConcurrentDictionary<string, Func<ExecutionContext, object, object>> RegisteredHandlers { get; } =
			new ConcurrentDictionary<string, Func<ExecutionContext, object, object>>();

		public virtual object Execute(string name, ExecutionContext context, object parameters)
		{
			if (RegisteredHandlers.TryGetValue(name, out var handler))
			{
				return handler(context, parameters);
			}

			throw new MethodNotFoundException(name);
		}

		public virtual void RegisterHandler(string name, Func<ExecutionContext, object, object> handler)
		{
			RegisteredHandlers[name ?? throw new ArgumentNullException(nameof(name))] =
				handler ?? throw new ArgumentNullException(nameof(handler));
		}
	}
}
