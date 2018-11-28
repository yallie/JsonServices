using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Events;
using JsonServices.Exceptions;

namespace JsonServices.Services
{
	public class ServiceExecutor : IServiceExecutor
	{
		public ServiceExecutor()
		{
			// built-in services: subscription/unsubscription
			RegisterHandler(SubscriptionMessage.MessageName, (ctx, param) =>
			{
				new SubscriptionService().Execute(ctx, (SubscriptionMessage)param);
				return null;
			});
		}

		private ConcurrentDictionary<string, Func<ServiceExecutionContext, object, object>> RegisteredHandlers { get; } =
			new ConcurrentDictionary<string, Func<ServiceExecutionContext, object, object>>();

		public virtual object Execute(string name, ServiceExecutionContext context, object parameters)
		{
			if (RegisteredHandlers.TryGetValue(name, out var handler))
			{
				return handler(context, parameters);
			}

			throw new MethodNotFoundException(name);
		}

		public virtual void RegisterHandler(string name, Func<ServiceExecutionContext, object, object> handler)
		{
			RegisteredHandlers[name ?? throw new ArgumentNullException(nameof(name))] =
				handler ?? throw new ArgumentNullException(nameof(handler));
		}
	}
}
