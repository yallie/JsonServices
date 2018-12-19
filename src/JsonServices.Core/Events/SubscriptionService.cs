using JsonServices.Services;

namespace JsonServices.Events
{
	internal class SubscriptionService
	{
		public void Execute(SubscriptionMessage message)
		{
			if (message?.Subscriptions != null)
			{
				foreach (var sub in message.Subscriptions)
				{
					if (sub.Enabled)
					{
						Subscribe(sub);
					}
					else
					{
						Unsubscribe(sub);
					}
				}
			}
		}

		private void Subscribe(SubscriptionMessage.Subscription message)
		{
			var context = RequestContext.Current;
			context.Server.SubscriptionManager.Add(new ServerSubscription
			{
				ConnectionId = context.ConnectionId,
				SubscriptionId = message.SubscriptionId,
				EventName = message.EventName,
				EventFilter = message.EventFilter,
			});
		}

		private void Unsubscribe(SubscriptionMessage.Subscription message)
		{
			var context = RequestContext.Current;
			context.Server.SubscriptionManager.Remove(message.EventName, context.ConnectionId, message.SubscriptionId);
		}
	}
}
