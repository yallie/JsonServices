using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices.Events
{
	public class ServerSubscriptionManager
	{
		public ServerSubscriptionManager(IServer server)
		{
			Server = server ?? throw new ArgumentNullException(nameof(server));
		}

		private IServer Server { get; set; }

		// eventName => connectionId => subscriptionId => subscription
		private ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ServerSubscription>>> Subscriptions { get; } =
			new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ServerSubscription>>>();

		private ConcurrentDictionary<string, ServerSubscription> GetSubscriptions(string eventName, string connectionId)
		{
			var eventSubs = Subscriptions.GetOrAdd(eventName, n => new ConcurrentDictionary<string, ConcurrentDictionary<string, ServerSubscription>>());
			return eventSubs.GetOrAdd(connectionId, n => new ConcurrentDictionary<string, ServerSubscription>());
		}

		public void Add(ServerSubscription sub)
		{
			var subs = GetSubscriptions(sub.EventName, sub.ConnectionId);
			subs[sub.SubscriptionId] = sub;
		}

		public void Remove(string eventName, string connectionId, string subscriptionId)
		{
			var subs = GetSubscriptions(eventName, connectionId);
			subs.TryRemove(subscriptionId, out var ignored);
		}

		public void Broadcast(string eventName, string data, EventArgs args)
		{
			// fire and forget
			BroadcastAsync(eventName, data, args);
		}

		internal Task BroadcastAsync(string eventName, string data, EventArgs args)
		{
			// all event's subscriptions: connectionId => subscriptionId => subscription
			var eventSubs = Subscriptions.GetOrAdd(eventName, n => new ConcurrentDictionary<string, ConcurrentDictionary<string, ServerSubscription>>());
			var sendTasks =
				from pair in eventSubs
				let connectionId = pair.Key
				let subscriptions = pair.Value.Values
				let conn = Server.TryGetConnection(connectionId)
				where conn != null // TODO: check for empty filter or a filter that accepts args
				where subscriptions.Any(s => s.EventFilter == null || !s.EventFilter.Any())
				select Server.SendAsync(connectionId, data);

			// TODO: decide where to handle exceptions
			return Task.WhenAll(sendTasks);
		}
	}
}
