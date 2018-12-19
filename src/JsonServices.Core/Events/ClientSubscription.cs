using System;
using System.Collections.Generic;

namespace JsonServices.Events
{
	internal class ClientSubscription<TEventArgs> : IClientSubscription
		where TEventArgs : EventArgs
	{
		public string SubscriptionId { get; set; }

		public string EventName { get; set; }

		public EventHandler<TEventArgs> EventHandler { get; set; }

		public Dictionary<string, string> EventFilter { get; set; }

		public void Invoke(object sender, EventArgs args)
		{
			if (EventHandler != null && EventFilter.Matches(args))
			{
				EventHandler?.Invoke(sender, (TEventArgs)args);
			}
		}

		private SubscriptionMessage.Subscription CreateSubscription()
		{
			return new SubscriptionMessage.Subscription
			{
				SubscriptionId = SubscriptionId,
				EventName = EventName,
				EventFilter = EventFilter,
				Enabled = true,
			};
		}

		public SubscriptionMessage CreateSubscriptionMessage()
		{
			return new SubscriptionMessage
			{
				Subscriptions = new[] { CreateSubscription() },
			};
		}

		public SubscriptionMessage CreateUnsubscriptionMessage()
		{
			var sub = CreateSubscription();
			sub.EventFilter = null; // no need to send the filter again
			sub.Enabled = false;

			return new SubscriptionMessage
			{
				Subscriptions = new[] { sub },
			};
		}
	}
}
