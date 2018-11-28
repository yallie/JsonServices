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
	public class JsonClientTests
	{
		[Test]
		public void JsonClientRequiresServices()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonClient(null, null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonClient(new StubClient(new StubServer()), null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonClient(new StubClient(new StubServer()), new StubMessageTypeProvider(), null));
		}

		private Task Timeout => Task.Delay(500);

		[Test]
		public async Task JsonClientSupportsSubscriptionsAndUnsubscriptions()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server, "jc");
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			var js = new JsonServer(server, provider, serializer, executor).Start();
			var jc = new JsonClient(client, provider, serializer);
			await jc.ConnectAsync();

			// second client
			var sc = new JsonClient(new StubClient(server, "sc"), provider, serializer);
			await sc.ConnectAsync();

			// subscribe to jc events
			var jcounter = 0;
			var jcancel = default(bool?);
			var jtcs = new TaskCompletionSource<bool>();
			var junsubscribe = await jc.Subscribe<CancelEventArgs>(
				EventBroadcaster.BeforeShutdownEventName, (s, e) =>
				{
					jcounter++;
					jcancel = e.Cancel;
					jtcs.TrySetResult(true);
				});

			// subscribe to sc events
			var scounter = 0;
			var spropName = default(string);
			var stcs = new TaskCompletionSource<bool>();
			var sunsubscribe = await sc.Subscribe<MyCoolEventArgs>(
				EventBroadcaster.AfterStartupEventName, (s, e) =>
				{
					scounter++;
					spropName = e.PropertyName;
					stcs.TrySetResult(true);
				});

			// call EventBroadcaster.AfterStartup
			await jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.AfterStartupEventName,
			});

			// sc is subscribed to AfterStartup event, jc is not
			await stcs.Task;
			Assert.AreEqual(1, scounter);
			Assert.AreEqual(0, jcounter);
			Assert.AreEqual(nameof(EventBroadcaster), spropName);

			// call EventBroadcaster.BeforeShutdown
			await jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.BeforeShutdownEventName,
			});

			// js is subscribed to BeforeShutdown event, sc is not
			await jtcs.Task;
			Assert.AreEqual(1, scounter);
			Assert.AreEqual(1, jcounter);
			Assert.IsTrue(jcancel);

			// restart both task completion sources
			jtcs = new TaskCompletionSource<bool>();
			stcs = new TaskCompletionSource<bool>();

			// call EventBroadcaster.BeforeShutdown
			await jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.BeforeShutdownEventName,
			});

			// js is subscribed to BeforeShutdown event, sc is not
			await jtcs.Task;
			Assert.AreEqual(1, scounter);
			Assert.AreEqual(2, jcounter);
			Assert.IsTrue(jcancel);

			// unsubscribe sc from AfterStartup event
			await sunsubscribe();

			// call EventBroadcaster.AfterStartup
			await jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.AfterStartupEventName,
			});

			// make sure that event is not handled anymore
			await Task.WhenAny(stcs.Task, Timeout);
			Assert.AreEqual(1, scounter);

			// unsubscribe jc from BeforeShutdown event
			await junsubscribe();
			jtcs = new TaskCompletionSource<bool>();
			scounter = 0;
			jcounter = 0;

			// call EventBroadcaster.BeforeShutdown
			await jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.BeforeShutdownEventName,
			});

			// nobody is subscribed to BeforeShutdown event
			await Task.WhenAny(jtcs.Task, Timeout);
			Assert.AreEqual(0, scounter);
			Assert.AreEqual(0, jcounter);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
			Assert.AreEqual(0, sc.PendingMessages.Count);
		}
	}
}
