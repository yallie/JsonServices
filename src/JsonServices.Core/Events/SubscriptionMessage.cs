using System.Collections.Generic;
using System.Linq;
using JsonServices.Messages;

namespace JsonServices.Events
{
	public class SubscriptionMessage : IReturnVoid, ICustomName
	{
		public const string MessageName = "rpc.subscription";

		string ICustomName.MessageName => MessageName;

		public class Subscription
		{
			public string SubscriptionId { get; set; }

			public bool Enabled { get; set; }

			public string EventName { get; set; }

			public Dictionary<string, string> EventFilter { get; set; }
		}

		public Subscription[] Subscriptions { get; set; }

		public override string ToString()
		{
			if (Subscriptions == null)
			{
				return base.ToString();
			}

			return string.Join(", ", Subscriptions.Select(s => s.EventName));
		}
	}
}
