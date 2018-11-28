using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Events
{
	public interface IClientSubscription
	{
		string SubscriptionId { get; set; }

		Delegate EventHandler { get; set; }

		void Invoke(object sender, EventArgs args);
	}
}
