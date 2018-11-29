using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Events;
using NUnit.Framework;

namespace JsonServices.Tests.Events
{
	[TestFixture]
	public class ClientSubscriptionManagerTests
	{
		private ClientSubscriptionManager SubscriptionManager { get; } =
			new ClientSubscriptionManager();

		private string EventName => "SomethingHappened";

		[Test]
		public void SubscribeAndUnsubscribeClientSubscription()
		{
			var firedCounter = 0;
			var cancel = false;
			void handler(object s, CancelEventArgs e)
			{
				firedCounter++;
				cancel = e.Cancel;
			}

			var subscription = new ClientSubscription<CancelEventArgs>
			{
				SubscriptionId = "1",
				EventHandler = handler,
				EventName = EventName,
			};

			var unsubscribe = SubscriptionManager.Add(subscription);
			Assert.AreEqual(0, firedCounter);
			Assert.IsFalse(cancel);

			SubscriptionManager.Broadcast(EventName, new CancelEventArgs(true));
			Assert.AreEqual(1, firedCounter);
			Assert.IsTrue(cancel);

			SubscriptionManager.Broadcast(EventName, new CancelEventArgs(false));
			Assert.AreEqual(2, firedCounter);
			Assert.IsFalse(cancel);

			unsubscribe();
			SubscriptionManager.Broadcast(EventName, new CancelEventArgs(true));

			// event wasn't fired
			Assert.AreEqual(2, firedCounter);
			Assert.IsFalse(cancel);
		}
	}
}
