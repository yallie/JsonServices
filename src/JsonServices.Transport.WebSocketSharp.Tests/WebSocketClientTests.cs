using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Transport.WebSocketSharp.Tests
{
	[TestFixture]
	public class WebSocketClientTests : TestFixtureBase
	{
		[Test]
		public async Task JsonClientSupportsSubscriptionsAndUnsubscriptions()
		{
			// websocket transport
			var server = new WebSocketServer("ws://localhost:8765");
			var client = new WebSocketClient("ws://localhost:8765");
			var secondClient = new WebSocketClient("ws://localhost:8765");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor).Start())
			using (var jc = new JsonClient(client, provider, serializer))
			using (var sc = new JsonClient(secondClient, provider, serializer))
			{
				// set names for easier debugging
				jc.DebugName = "First";
				sc.DebugName = "Second";

				// connect both clients
				await jc.ConnectAsync();
				await sc.ConnectAsync();

				// subscribe to jc events
				var jcounter = 0;
				var jcancel = default(bool?);
				var jtcs = new TaskCompletionSource<bool>();
				var junsubscribe = await Assert_NotTimedOut(jc.Subscribe<CancelEventArgs>(
					EventBroadcaster.BeforeShutdownEventName, (s, e) =>
					{
						jcounter++;
						jcancel = e.Cancel;
						jtcs.TrySetResult(true);
					}), "jc.Subscribe<CancelEventArgs>(...)");

				// subscribe to sc events
				var scounter = 0;
				var spropName = default(string);
				var stcs = new TaskCompletionSource<bool>();
				var sunsubscribe = await Assert_NotTimedOut(sc.Subscribe<MyCoolEventArgs>(
					EventBroadcaster.AfterStartupEventName, (s, e) =>
					{
						scounter++;
						spropName = e.PropertyName;
						stcs.TrySetResult(true);
					}), "sc.Subscribe<MyCoolEventArgs>(...)");

				// call EventBroadcaster.AfterStartup
				await Assert_NotTimedOut(jc.Call(new EventBroadcaster
				{
					EventName = EventBroadcaster.AfterStartupEventName,
				}), "jc.Call(new EventBroadcaster...AfterStartup))");

				// sc is subscribed to AfterStartup event, jc is not
				await Assert_NotTimedOut(stcs.Task, "stcs.Task");
				Assert.AreEqual(1, scounter);
				Assert.AreEqual(0, jcounter);
				Assert.AreEqual(nameof(EventBroadcaster), spropName);

				// call EventBroadcaster.BeforeShutdown
				await Assert_NotTimedOut(jc.Call(new EventBroadcaster
				{
					EventName = EventBroadcaster.BeforeShutdownEventName,
				}), "jc.Call(new EventBroadcaster(...BeforeShutdown))");

				// js is subscribed to BeforeShutdown event, sc is not
				await Assert_NotTimedOut(jtcs.Task, "jtcs.Task");
				Assert.AreEqual(1, scounter);
				Assert.AreEqual(1, jcounter);
				Assert.IsTrue(jcancel);

				// restart both task completion sources
				jtcs = new TaskCompletionSource<bool>();
				stcs = new TaskCompletionSource<bool>();

				// call EventBroadcaster.BeforeShutdown
				await Assert_NotTimedOut(jc.Call(new EventBroadcaster
				{
					EventName = EventBroadcaster.BeforeShutdownEventName,
				}), "jc.Call(new EventBroadcaster(...BeforeShutdown)) #2");

				// js is subscribed to BeforeShutdown event, sc is not
				await Assert_NotTimedOut(jtcs.Task, "jtcs.Task #2");
				Assert.AreEqual(1, scounter);
				Assert.AreEqual(2, jcounter);
				Assert.IsTrue(jcancel);

				// unsubscribe sc from AfterStartup event
				await Assert_NotTimedOut(sunsubscribe(), "sunsubscribe()");

				// call EventBroadcaster.AfterStartup
				await Assert_NotTimedOut(jc.Call(new EventBroadcaster
				{
					EventName = EventBroadcaster.AfterStartupEventName,
				}), "jc.Call(new EventBroadcaster(...AfterStartup)) #2");

				// make sure that event is not handled anymore
				await Assert_TimedOut(stcs.Task, "stcs.Task #2");
				Assert.AreEqual(1, scounter);

				// unsubscribe jc from BeforeShutdown event
				await Assert_NotTimedOut(junsubscribe(), "junsubscribe()");
				jtcs = new TaskCompletionSource<bool>();
				scounter = 0;
				jcounter = 0;

				// call EventBroadcaster.BeforeShutdown
				await Assert_NotTimedOut(jc.Call(new EventBroadcaster
				{
					EventName = EventBroadcaster.BeforeShutdownEventName,
				}), "jc.Call(new EventBroadcaster(...BeforeShutdown)) #3");

				// nobody is subscribed to BeforeShutdown event
				await Assert_TimedOut(jtcs.Task, "jtcs.Task #3");
				Assert.AreEqual(0, scounter);
				Assert.AreEqual(0, jcounter);

				// make sure all incoming messages are processed
				Assert.AreEqual(0, jc.PendingMessages.Count);
				Assert.AreEqual(0, sc.PendingMessages.Count);
			}
		}
	}
}
