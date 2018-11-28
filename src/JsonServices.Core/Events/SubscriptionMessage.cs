using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;

namespace JsonServices.Events
{
	public class SubscriptionMessage : IReturnVoid, ICustomName
	{
		public const string MessageName = "rpc.subscription";

		string ICustomName.MessageName => MessageName;

		public bool Enabled { get; set; }

		public string EventName { get; set; }

		public Dictionary<string, string> EventFilter { get; set; }

		public string SubscriptionId { get; set; }
	}
}
