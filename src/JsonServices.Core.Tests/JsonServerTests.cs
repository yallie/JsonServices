using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;
using Serializer = JsonServices.Serialization.ServiceStack.Serializer;

namespace JsonServices.Tests
{
	[TestFixture]
	public class JsonServerTests
	{
		[Test]
		public void JsonServerRequiresServices()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonServer(null, null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), new Serializer(null), null));
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), new Serializer(new StubMessageTypeProvider()), null));
		}

		[Test]
		public async Task CallGetVersionService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var locator = new StubMessageTypeProvider();
			var serializer = new Serializer(locator);
			var executor = new StubExecutor();

			// json server and client
			var js = new JsonServer(server, serializer, executor).Start();
			var jc = new JsonClient(client, serializer);
			await jc.ConnectAsync();

			// call GetVersion
			var msg = new GetVersion();
			var result = await jc.Call(msg);
			Assert.NotNull(result);
			Assert.AreEqual("0.01-alpha", result.Version);

			// call GetVersion
			msg = new GetVersion { IsInternal = true };
			result = await jc.Call(msg);
			Assert.NotNull(result);
			Assert.AreEqual("Version 0.01-alpha, build 12345, by yallie", result.Version);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
		}

		[Test]
		public async Task CallCalculateService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var locator = new StubMessageTypeProvider();
			var serializer = new Serializer(locator);
			var executor = new StubExecutor();

			// json server and client
			var js = new JsonServer(server, serializer, executor).Start();
			var jc = new JsonClient(client, serializer);
			await jc.ConnectAsync();

			// normal call
			var msg = new Calculate
			{
				FirstOperand = 353,
				SecondOperand = 181,
				Operation = "+",
			};

			var result = await jc.Call(msg);
			Assert.NotNull(result);
			Assert.AreEqual(534, result.Result);

			msg.SecondOperand = 333;
			result = await jc.Call(msg);
			Assert.NotNull(result);
			Assert.AreEqual(686, result.Result);

			// call with error
			msg.Operation = "#";
			Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));

			// call with another error
			msg.Operation = "%";
			msg.SecondOperand = 0;
			Assert.ThrowsAsync<JsonServicesException>(async () => await jc.Call(msg));

			// normal call again
			msg.Operation = "*";
			result = await jc.Call(msg);
			Assert.NotNull(result);
			Assert.AreEqual(0, result.Result);

			// make sure all incoming messages are processed
			Assert.AreEqual(0, jc.PendingMessages.Count);
		}
	}
}
