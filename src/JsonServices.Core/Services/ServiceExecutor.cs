using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Auth;
using JsonServices.Events;
using JsonServices.Exceptions;

namespace JsonServices.Services
{
	public class ServiceExecutor : IServiceExecutor
	{
		public ServiceExecutor()
		{
			// built-in services: authentication
			RegisterHandler(AuthRequest.MessageName, (ctx, param) =>
			{
				return new AuthService().Authenticate(ctx, (AuthRequest)param);
			});

			// subscription/unsubscription
			RegisterHandler(SubscriptionMessage.MessageName, (ctx, param) =>
			{
				new SubscriptionService().Execute(ctx, (SubscriptionMessage)param);
				return null;
			});
		}

		private ConcurrentDictionary<string, Func<IRequestContext, object, object>> RegisteredHandlers { get; } =
			new ConcurrentDictionary<string, Func<IRequestContext, object, object>>();

		protected virtual bool IsAuthenticationRequired(string name, IRequestContext context, object parameters)
		{
			return name != AuthRequest.MessageName;
		}

		protected virtual void CheckAuthentication(string name, IRequestContext context, object parameters)
		{
			if (context.Connection.CurrentUser == null)
			{
				throw new AuthRequiredException(name);
			}
		}

		public virtual object Execute(string name, IRequestContext context, object parameters)
		{
			if (IsAuthenticationRequired(name, context, parameters))
			{
				CheckAuthentication(name, context, parameters);
			}

			if (RegisteredHandlers.TryGetValue(name, out var handler))
			{
				return handler(context, parameters);
			}

			throw new MethodNotFoundException(name);
		}

		public virtual void RegisterHandler(string name, Func<IRequestContext, object, object> handler)
		{
			RegisteredHandlers[name ?? throw new ArgumentNullException(nameof(name))] =
				handler ?? throw new ArgumentNullException(nameof(handler));
		}
	}
}
