using System;
using System.Collections.Concurrent;
using System.Linq;
using JsonServices.Auth;
using JsonServices.Events;
using JsonServices.Exceptions;
using JsonServices.Sessions;

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

			RegisterHandler(LogoutMessage.MessageName, param =>
			{
				new LogoutService().Execute((LogoutMessage)param);
				return null;
			});

			// subscription/unsubscription
			RegisterHandler(SubscriptionMessage.MessageName, param =>
			{
				new SubscriptionService().Execute((SubscriptionMessage)param);
				return null;
			});

			RegisterHandler(VersionRequest.MessageName, param =>
			{
				return new VersionService().Execute((VersionRequest)param);
			});
		}

		private ConcurrentDictionary<string, Func<object, object>> RegisteredHandlers { get; } =
			new ConcurrentDictionary<string, Func<object, object>>();

		protected virtual bool IsAuthenticationRequired(string name, object parameters)
		{
			// login and logout messages doesn't need any authentication
			var allowed = new[] { AuthRequest.MessageName, LogoutMessage.MessageName };
			return !allowed.Contains(name);
		}

		protected virtual void CheckAuthentication(string name, object parameters)
		{
			var context = RequestContext.Current;
			if (context?.Connection?.Session?.CurrentUser == null)
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

			// renew current session timestamp on every call
			if (RequestContext.Current != null)
			{
				var sessionId = RequestContext.Current.Connection?.Session?.SessionId;
				var sessionManager = RequestContext.Current.Server.SessionManager;
				sessionManager.RenewSession(sessionId);
			}

			// execute the requested service
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
