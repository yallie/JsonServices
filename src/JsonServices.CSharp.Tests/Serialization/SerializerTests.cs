using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Tests.Messages;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public class SerializerTests
	{
		private ISerializer Serializer { get; } = new Serializer(new StubLocator());

		[Test]
		public void SerializerSerializesRequestOneWayMessage()
		{
			var msg = new RequestMessage<GetVersion>
			{
				Params = new GetVersion
				{
					IsInternal = true
				}
			};

			var data = Serializer.Serialize(msg);
			var payload = Encoding.UTF8.GetString(data);

			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true}}", payload);
		}

		[Test]
		public void SerializerSerializesRequestMessage()
		{
			var msg = new RequestMessage<GetVersion>
			{
				Id = "123",
				Params = new GetVersion
				{
					IsInternal = true
				}
			};

			var data = Serializer.Serialize(msg);
			var payload = Encoding.UTF8.GetString(data);

			Assert.NotNull(payload);
			Assert.AreEqual("{\"jsonrpc\":\"2.0\",\"method\":\"JsonServices.Tests.Messages.GetVersion\",\"params\":{\"IsInternal\":true},\"id\":\"123\"}", payload);
		}
	}
}
