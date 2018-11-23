using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			Assert.Throws<ArgumentNullException>(() => new JsonClient(null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonClient(new StubClient(new StubServer()), null));
		}
	}
}
