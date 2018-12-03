using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class JsonClientTests : TestFixtureBase
	{
		[Test]
		public void JsonClientRequiresServices()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonClient(null, null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonClient(new StubClient(new StubServer()), null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonClient(new StubClient(new StubServer()), new StubMessageTypeProvider(), null));
		}

		[Test]
		public async Task JsonClientSupportsSubscriptionsAndUnsubscriptions()
		{
			// fake transport and serializer
			var server = new StubServer();
			var serverSerializer = new Serializer();
			var serverProvider = new StubMessageTypeProvider();
			var executor = new StubExecutor();

			var client = new StubClient(server, "jc");
			var clientProvider = new StubMessageTypeProvider();
			var clientSerializer = new Serializer();

			// json server and client
			var js = new JsonServer(server, serverProvider, serverSerializer, executor);
			var jc = new JsonClient(client, clientProvider, clientSerializer);

			// second client
			var secondClientProvider = new StubMessageTypeProvider();
			var secondClientSerializer = new Serializer();
			var sc = new JsonClient(new StubClient(server, "sc"), secondClientProvider, secondClientSerializer);

			// test core
			await TestSubscriptionsAndUnsubscriptionsCore(js, jc, sc);
		}

		protected async Task TestSubscriptionsAndUnsubscriptionsCore(JsonServer js, JsonClient jc, JsonClient sc)
		{
			// unhandled exception handlers
			var connected = 0;
			var disconnected = 0;
			js.ClientConnected += (s, e) => connected++;
			js.ClientDisconnected += (s, e) => disconnected++;
			js.UnhandledException += (s, e) => Assert.Fail($"Unhandled server exception: {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");
			jc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception in jc (first client): {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");
			sc.UnhandledException += (s, e) => Assert.Fail($"Unhandled client exception in sc (second client): {e.Exception}. Connected: {connected}, disconnected: {disconnected}.");

			// start json server and connect both clients
			js.Start();
			await Assert_NotTimedOut(jc.ConnectAsync(), "jc.ConnectAsync()");
			await Assert_NotTimedOut(sc.ConnectAsync(), "sc.ConnectAsync()");

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
			await Assert_TimedOut(stcs.Task, "stcs.Task #2", Task.Delay(500));
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
			await Assert_TimedOut(jtcs.Task, "jtcs.Task #3", Task.Delay(500));
			Assert.AreEqual(0, scounter);
			Assert.AreEqual(0, jcounter);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
			Assert.AreEqual(0, sc.PendingMessages.Count);
		}
	}
}
