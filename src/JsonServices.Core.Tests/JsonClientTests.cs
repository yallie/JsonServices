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

		[Test, Ignore("TODO: not implemented yet")]
		public async Task JsonClientSupportsSubscriptions()
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
			var junsubscribe = await jc.Subscribe<CancelEventArgs>(
				EventBroadcaster.BeforeShutdownEventName, (s, e) =>
				{
					jcounter++;
					jcancel = e.Cancel;
				});

			// subscribe to sc events
			var scounter = 0;
			var spropName = default(string);
			var sunsubscribe = await sc.Subscribe<PropertyChangedEventArgs>(
				EventBroadcaster.AfterStartupEventName, (s, e) =>
				{
					scounter++;
					spropName = e.PropertyName;
				});

			// call EventBroadcaster
			await jc.Call(new EventBroadcaster
			{
				EventName = EventBroadcaster.AfterStartupEventName
			});

			/*/ sc is subscribed to AfterStartup event
			Assert.AreEqual(1, scounter);
			Assert.AreEqual(0, jcounter);
			Assert.NotNull(result);
			Assert.AreEqual("0.01-alpha", result.Version);

			// call GetVersion
			msg = new GetVersion { IsInternal = true };
			result = await jc.Call(msg);
			Assert.NotNull(result);
			Assert.AreEqual("Version 0.01-alpha, build 12345, by yallie", result.Version);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);*/
		}
	}
}
