using System;
using System.Collections.Concurrent;
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
			RegisterHandler(AuthRequest.MessageName, param =>
			{
				return new AuthService().Authenticate((AuthRequest)param);
			});

			// subscription/unsubscription
			RegisterHandler(SubscriptionMessage.MessageName, param =>
			{
				new SubscriptionService().Execute((SubscriptionMessage)param);
				return null;
			});
		}

		private ConcurrentDictionary<string, Func<object, object>> RegisteredHandlers { get; } =
			new ConcurrentDictionary<string, Func<object, object>>();

		protected virtual bool IsAuthenticationRequired(string name, object parameters)
		{
			return name != AuthRequest.MessageName;
		}

		protected virtual void CheckAuthentication(string name, object parameters)
		{
			var context = RequestContext.Current;
			if (context.Connection.Session?.CurrentUser == null)
			{
				throw new AuthRequiredException(name);
			}
		}

		public virtual object Execute(string name, object parameters)
		{
			if (IsAuthenticationRequired(name, parameters))
			{
				CheckAuthentication(name, parameters);
			}

			if (RegisteredHandlers.TryGetValue(name, out var handler))
			{
				return handler(parameters);
			}

			throw new MethodNotFoundException(name);
		}

		public virtual void RegisterHandler(string name, Func<object, object> handler)
		{
			RegisteredHandlers[name ?? throw new ArgumentNullException(nameof(name))] =
				handler ?? throw new ArgumentNullException(nameof(handler));
		}
	}
}
