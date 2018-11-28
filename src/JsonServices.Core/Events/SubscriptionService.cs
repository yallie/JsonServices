using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Services;

namespace JsonServices.Events
{
	public class SubscriptionService
	{
		public void Execute(ServiceExecutionContext context, SubscriptionMessage message)
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

		private void Subscribe(ServiceExecutionContext context, SubscriptionMessage message)
		{
			context.Server.SubscriptionManager.Add(new ServerSubscription
			{
				ConnectionId = context.ConnectionId,
				SubscriptionId = message.SubscriptionId,
				EventName = message.EventName,
				EventFilter = message.EventFilter,
			});
		}

		private void Unsubscribe(ServiceExecutionContext context, SubscriptionMessage message)
		{
			context.Server.SubscriptionManager.Remove(message.EventName, context.ConnectionId, message.SubscriptionId);
		}
	}
}
