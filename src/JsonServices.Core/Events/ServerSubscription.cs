using System.Collections.Generic;

namespace JsonServices.Events
{
	public class ServerSubscription
	{
		public string ConnectionId { get; set; }

		public string SubscriptionId { get; set; }

		public string EventName { get; set; }

		public Dictionary<string, string> EventFilter { get; set; }
	}
}
