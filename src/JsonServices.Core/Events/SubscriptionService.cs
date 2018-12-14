using JsonServices.Services;

namespace JsonServices.Events
{
	internal class SubscriptionService
	{
		public void Execute(SubscriptionMessage message)
		{
			if (message.Enabled)
			{
				Subscribe(message);
			}
			else
			{
				Unsubscribe(message);
			}
		}

		private void Subscribe(SubscriptionMessage message)
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

		private void Unsubscribe(SubscriptionMessage message)
		{
			var context = RequestContext.Current;
			context.Server.SubscriptionManager.Remove(message.EventName, context.ConnectionId, message.SubscriptionId);
		}
	}
}
