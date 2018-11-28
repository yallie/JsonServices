using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JsonServices.Events
{
	public class ClientSubscriptionManager
	{
		private ConcurrentDictionary<string, ConcurrentDictionary<string, IClientSubscription>> Subscriptions { get; } =
			new ConcurrentDictionary<string, ConcurrentDictionary<string, IClientSubscription>>();

		public Action Add<TEventArgs>(ClientSubscription<TEventArgs> sub)
			where TEventArgs : EventArgs
		{
			var subs = Subscriptions.GetOrAdd(sub.EventName, n => new ConcurrentDictionary<string, IClientSubscription>());
			subs[sub.SubscriptionId] = sub;
			return () => subs.TryRemove(sub.SubscriptionId, out var ignored);
		}

		public void Broadcast(string eventName, EventArgs eventArgs)
		{
			if (Subscriptions.TryGetValue(eventName, out var list))
			{
				foreach (var sub in list.Values)
				{
					sub.Invoke(this, eventArgs);
				}
			}
		}

		public void BroadcastAsync(string eventName, EventArgs eventArgs)
		{
			ThreadPool.QueueUserWorkItem(x =>
			{
				try
				{
					Broadcast(eventName, eventArgs);
				}
				catch
				{
					// TODO: handle exceptions
				}
			});
		}
	}
}
