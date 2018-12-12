using JsonServices.Services;

namespace JsonServices.Events
{
	public class SubscriptionService
	{
		public void Execute(RequestContext context, SubscriptionMessage message)
		{
			if (message.Enabled)
			{
				Subscribe(context, message);
			}
			else
			{
				Unsubscribe(context, message);
			}
		}

		private void Subscribe(RequestContext context, SubscriptionMessage message)
		{
			context.Server.SubscriptionManager.Add(new ServerSubscription
			{
				ConnectionId = context.ConnectionId,
				SubscriptionId = message.SubscriptionId,
				EventName = message.EventName,
				EventFilter = message.EventFilter,
			});
		}

		private void Unsubscribe(RequestContext context, SubscriptionMessage message)
		{
			context.Server.SubscriptionManager.Remove(message.EventName, context.ConnectionId, message.SubscriptionId);
		}
	}
}
