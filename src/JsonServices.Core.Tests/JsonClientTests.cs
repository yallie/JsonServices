using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using JsonServices.Auth;
using JsonServices.Exceptions;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Services;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using JsonServices.Transport;
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
		public void JsonClientCallAndNotifyThrowOnNullArguments()
		{
			var server = new StubServer();
			var client = new StubClient(server);
			var clientProvider = new StubMessageTypeProvider();
			var clientSerializer = new Serializer();
			var jc = new JsonClient(client, clientProvider, clientSerializer);

			Assert.Throws<ArgumentNullException>(() => jc.Notify(null));
			Assert.ThrowsAsync<ArgumentNullException>(() => jc.Call(null));
			Assert.ThrowsAsync<ArgumentNullException>(() => jc.Call<string>(null));
		}

		[Test]
		public void JsonClientCallsUnhandledException()
		{
			var server = new StubServer();
			var client = new StubClient(server);
			var clientProvider = new StubMessageTypeProvider();
			var clientSerializer = new Serializer();
			var jc = new JsonClient(client, clientProvider, clientSerializer);
			var exception = default(Exception);
			jc.UnhandledException += (s, e) => exception = e.Exception;

			// simulate JsonServicesException
			jc.HandleClientMessage(this, new MessageEventArgs());
			Assert.IsInstanceOf<JsonServicesException>(exception);

			exception = null;
			jc.HandleClientMessage(this, new MessageEventArgs { Data = string.Empty });
			Assert.IsInstanceOf<JsonServicesException>(exception);

			// simulate NullReferenceException
			exception = null;
			jc.Serializer = null;
			jc.HandleClientMessage(this, new MessageEventArgs { Data = "Goofy" });
			Assert.IsInstanceOf<NullReferenceException>(exception);
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

		protected async Task TestSubscriptionsAndUnsubscriptionsCore(JsonServer js, JsonClient jc, JsonClient sc, ICredentials credentials = null)
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
			await Assert_NotTimedOut(jc.ConnectAsync(credentials), "jc.ConnectAsync()");
			await Assert_NotTimedOut(sc.ConnectAsync(credentials), "sc.ConnectAsync()");

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

			// one-way call EventBroadcaster.AfterStartup
			jc.Notify(new EventBroadcaster
			{
				EventName = EventBroadcaster.AfterStartupEventName,
			});

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
			await Assert_TimedOut(stcs.Task, "stcs.Task #2", Task.Delay(200));
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
			await Assert_TimedOut(jtcs.Task, "jtcs.Task #3", Task.Delay(200));
			Assert.AreEqual(0, scounter);
			Assert.AreEqual(0, jcounter);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
			Assert.AreEqual(0, sc.PendingMessages.Count);
		}

		[Test]
		public async Task JsonClientSupportsFilteredSubscriptionsAndUnsubscriptions()
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
			await TestFilteredSubscriptionsAndUnsubscriptionsCore(js, jc, sc);
		}

		protected async Task TestFilteredSubscriptionsAndUnsubscriptionsCore(JsonServer js, JsonClient jc, JsonClient sc, ICredentials credentials = null)
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
			await Assert_NotTimedOut(jc.ConnectAsync(credentials), "jc.ConnectAsync()");
			await Assert_NotTimedOut(sc.ConnectAsync(credentials), "sc.ConnectAsync()");

			// subscribe to jc events
			var jcounter = 0;
			var jeventArgs = default(FilteredEventArgs);
			var jtcs = new TaskCompletionSource<bool>();
			var junsubscribe = await Assert_NotTimedOut(jc.Subscribe<FilteredEventArgs>(
				EventBroadcaster.FilteredEventName, (s, e) =>
				{
					jcounter++;
					jeventArgs = e;
					jtcs.TrySetResult(true);
				},
				new Dictionary<string, string>
				{
					{ nameof(FilteredEventArgs.StringProperty), "Twain" }
				}), "jc.Subscribe<FilteredEventArgs>(...)");

			// subscribe to sc events
			var scounter = 0;
			var seventArgs = default(FilteredEventArgs);
			var stcs = new TaskCompletionSource<bool>();
			var sunsubscribe = await Assert_NotTimedOut(sc.Subscribe<FilteredEventArgs>(
				EventBroadcaster.FilteredEventName, (s, e) =>
				{
					scounter++;
					seventArgs = e;
					stcs.TrySetResult(true);
				},
				new Dictionary<string, string>
				{
					{ nameof(FilteredEventArgs.StringProperty), "Mark" }
				}), "sc.Subscribe<FilteredEventArgs>(...)");

			// call EventBroadcaster.FilteredEvent
			await Assert_NotTimedOut(jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.FilteredEventName,
				StringArgument = "Mark Hamill",
			}), "jc.Call(new EventBroadcaster...FilteredEvent...Mark Hamill))");

			// sc is subscribed to Mark filtered event, jc is not
			await Assert_NotTimedOut(stcs.Task, "stcs.Task");
			Assert.AreEqual(1, scounter);
			Assert.AreEqual(0, jcounter);
			Assert.AreEqual("Mark Hamill", seventArgs.StringProperty);

			// restart both task completion sources
			jtcs = new TaskCompletionSource<bool>();
			stcs = new TaskCompletionSource<bool>();

			// call EventBroadcaster.FilteredEvent
			await Assert_NotTimedOut(jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.FilteredEventName,
				StringArgument = "Mark Twain",
			}), "jc.Call(new EventBroadcaster(...FilteredEvent...Mark Twain))");

			// js and sc are both subscribed to this filtered event
			await Assert_NotTimedOut(jtcs.Task, "jtcs.Task");
			await Assert_NotTimedOut(stcs.Task, "stcs.Task");
			Assert.AreEqual(2, scounter);
			Assert.AreEqual(1, jcounter);
			Assert.AreEqual("Mark Twain", jeventArgs.StringProperty);
			Assert.AreEqual("Mark Twain", seventArgs.StringProperty);

			// restart both task completion sources
			jtcs = new TaskCompletionSource<bool>();
			stcs = new TaskCompletionSource<bool>();

			// call EventBroadcaster.FilteredEvent
			await Assert_NotTimedOut(sc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.FilteredEventName,
				StringArgument = "TWAIN driver"
			}), "sc.Call(new EventBroadcaster(...FilteredEvent...TWAIN driver))");

			// jc is subscribed to TWAIN filtered event, sc is not
			await Assert_NotTimedOut(jtcs.Task, "jtcs.Task #2");
			Assert.AreEqual(2, scounter);
			Assert.AreEqual(2, jcounter);
			Assert.AreEqual("TWAIN driver", jeventArgs.StringProperty);

			// unsubscribe sc from the filtered event
			await Assert_NotTimedOut(sunsubscribe(), "sunsubscribe()");

			// one-way call EventBroadcaster.FilteredEvent
			jc.Notify(new EventBroadcaster
			{
				EventName = EventBroadcaster.FilteredEventName,
				StringArgument = "Mark Knopfler"
			});

			// make sure that event is not handled anymore
			await Assert_TimedOut(stcs.Task, "stcs.Task #2", Task.Delay(200));
			Assert.AreEqual(2, scounter);
			Assert.AreEqual(2, jcounter);

			// unsubscribe jc from the filtered event
			await Assert_NotTimedOut(junsubscribe(), "junsubscribe()");
			jtcs = new TaskCompletionSource<bool>();
			scounter = 0;
			jcounter = 0;

			// call EventBroadcaster.FilteredEvent
			await Assert_NotTimedOut(sc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.FilteredEventName,
				StringArgument = "Twain, Mark",
			}), "sc.Call(new EventBroadcaster(...FilteredEvent...Twain, Mark))");

			// nobody is subscribed to BeforeShutdown event
			await Assert_TimedOut(jtcs.Task, "jtcs.Task #3", Task.Delay(200));
			Assert.AreEqual(0, scounter);
			Assert.AreEqual(0, jcounter);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
			Assert.AreEqual(0, sc.PendingMessages.Count);
		}

		[Test]
		public async Task JsonClientCanDisconnectAndReconnect()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDisconnectAndReconnectCore(js, jc);
			}
		}

		protected async Task CallDisconnectAndReconnectCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			js.ProductName = "My awesome server";
			js.ProductVersion = "1.2.3.4";
			js.Start();

			// connect, call, disconnect
			await Assert_NotTimedOut(jc.ConnectAsync(credentials), "connect");

			var result = await Assert_NotTimedOut(jc.Call(new VersionRequest()), "jc.Call(VersionRequest)");
			Assert.NotNull(result);
			Assert.AreEqual(js.ProductName, result.ProductName);
			Assert.AreEqual(js.ProductVersion, result.ProductVersion);
			await Assert_NotTimedOut(jc.DisconnectAsync(), "disconnect");

			// reconnect, call, disconnect
			await Assert_NotTimedOut(jc.ConnectAsync(credentials), "reconnect");

			result = await Assert_NotTimedOut(jc.Call(new VersionRequest()), "jc.Call(VersionRequest) after reconnect");
			Assert.NotNull(result);
			Assert.AreEqual(js.ProductName, result.ProductName);
			Assert.AreEqual(js.ProductVersion, result.ProductVersion);
			await Assert_NotTimedOut(jc.DisconnectAsync(), "disconnect completely");
			Assert.AreEqual(0, jc.PendingMessages.Count);
		}

		[Test]
		public async Task JsonClientRejectsPendingMessagesWhenDisconnected()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDelayServiceAndDisconnectCore(js, jc);
			}
		}

		protected async Task CallDelayServiceAndDisconnectCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			js.Start();
			await jc.ConnectAsync(credentials);

			var call = jc.Call(new DelayRequest { Milliseconds = 1000 });
			await Task.Delay(100); // make sure that the call was actually sent
			await jc.DisconnectAsync();

			Assert.ThrowsAsync<ClientDisconnectedException>(async () =>
				await Assert_NotTimedOut(call, "jc.Call(Delay 1000)", Task.Delay(2000)));
		}

		[Test]
		public async Task JsonClientRejectsPendingMessagesWhenConnectionIsAborted()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var serializer = new Serializer();
			var executor = new StubExecutor();
			var provider = new StubMessageTypeProvider();

			// json server and client
			using (var js = new JsonServer(server, provider, serializer, executor))
			using (var jc = new JsonClient(client, provider, serializer))
			{
				await CallDelayServiceAndAbortConnectionCore(js, jc);
			}
		}

		protected async Task CallDelayServiceAndAbortConnectionCore(JsonServer js, JsonClient jc, ICredentials credentials = null)
		{
			js.Start();
			await jc.ConnectAsync(credentials);

			var call = jc.Call(new DelayRequest { Milliseconds = 1000 });
			await Task.Delay(100); // make sure that the call was actually sent
			await jc.Client.DisconnectAsync(); // should fire Disconnected event

			Assert.ThrowsAsync<ClientDisconnectedException>(async () =>
				await Assert_NotTimedOut(call, "jc.Call(Delay 1000)", Task.Delay(2000)));
		}
	}
}
