using System;
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

		public EventHandler<TEventArgs> EventHandler { get; set; }

		Delegate IClientSubscription.EventHandler
		{
			get { return EventHandler; }
			set { EventHandler = (EventHandler<TEventArgs>)value; }
		}

		public void Invoke(object sender, EventArgs args)
		{
			EventHandler?.Invoke(sender, (TEventArgs)args);
		}
	}
}
