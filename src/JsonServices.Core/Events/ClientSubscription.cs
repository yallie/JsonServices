﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Events
{
	public class ClientSubscription<TEventArgs> : IClientSubscription
		where TEventArgs : EventArgs
	{
		public string SubscriptionId { get; set; }

		public string EventName { get; set; }

		public EventHandler<TEventArgs> EventHandler { get; set; }

		public Dictionary<string, string> EventFilter { get; set; }

		Delegate IClientSubscription.EventHandler
		{
			get { return EventHandler; }
			set { EventHandler = (EventHandler<TEventArgs>)value; }
		}

		public void Invoke(object sender, EventArgs args)
		{
			EventHandler?.Invoke(sender, (TEventArgs)args);
		}

		public SubscriptionMessage CreateSubscriptionMessage()
		{
			return new SubscriptionMessage
			{
				SubscriptionId = SubscriptionId,
				EventName = EventName,
				EventFilter = EventFilter,
				Enabled = true,
			};
		}

		public SubscriptionMessage CreateUnsubscriptionMessage()
		{
			var msg = CreateSubscriptionMessage();
			msg.EventFilter = null; // no need to send the filter again
			msg.Enabled = false;
			return msg;
		}
	}
}