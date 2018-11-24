﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Serialization;
using JsonServices.Tests.Services;
using JsonServices.Tests.Transport;
using NUnit.Framework;

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
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), new Serializer(new StubLocator()), null));
		}

		[Test]
		public async Task CallGetVersionService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var locator = new StubLocator();
			var serializer = new Serializer(locator);
			var executor = new StubExecutor();

			// json server and client
			var js = new JsonServer(server, serializer, executor);
			var jc = new JsonClient(client, serializer);

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
		}

		[Test]
		public async Task CallCalculateService()
		{
			// fake transport and serializer
			var server = new StubServer();
			var client = new StubClient(server);
			var locator = new StubLocator();
			var serializer = new Serializer(locator);
			var executor = new StubExecutor();

			// json server and client
			var js = new JsonServer(server, serializer, executor);
			var jc = new JsonClient(client, serializer);

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
		}
	}
}
