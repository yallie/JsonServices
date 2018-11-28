using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
