using System;

namespace JsonServices.Events
{
	public interface IClientSubscription
	{
		string SubscriptionId { get; set; }

		void Invoke(object sender, EventArgs args);
	}
}
