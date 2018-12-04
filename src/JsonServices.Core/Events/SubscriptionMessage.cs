using System.Collections.Generic;
using JsonServices.Messages;

namespace JsonServices.Events
{
	public class SubscriptionMessage : IReturnVoid, ICustomName
	{
		public const string MessageName = "rpc.subscription";

		string ICustomName.MessageName => MessageName;

		public string SubscriptionId { get; set; }

		public bool Enabled { get; set; }

		public string EventName { get; set; }

		public Dictionary<string, string> EventFilter { get; set; }
	}
}
