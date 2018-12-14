using System;

namespace JsonServices.Events
{
	internal interface IClientSubscription
	{
		string SubscriptionId { get; set; }

		void Invoke(object sender, EventArgs args);
	}
}
