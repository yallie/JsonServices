using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Events
{
	public class ServerSubscriptionManager
	{
		// eventName => connectionId => subscriptionId => subscription
		private ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ServerSubscription>>> Subscriptions { get; } =
			new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ServerSubscription>>>();

		private ConcurrentDictionary<string, ServerSubscription> GetSubscriptions(string eventName, string connectionId)
		{
			var connSubs = Subscriptions.GetOrAdd(eventName, n => new ConcurrentDictionary<string, ConcurrentDictionary<string, ServerSubscription>>());
			return connSubs.GetOrAdd(connectionId, n => new ConcurrentDictionary<string, ServerSubscription>());
		}

		public void Add(string connectionId, ServerSubscription sub)
		{
			var subs = GetSubscriptions(sub.EventName, connectionId);
			subs[sub.SubscriptionId] = sub;
		}

		public void Remove(string eventName, string connectionId, string subscriptionId)
		{
			var subs = GetSubscriptions(eventName, connectionId);
			subs.TryRemove(subscriptionId, out var ignored);
		}
	}
}
