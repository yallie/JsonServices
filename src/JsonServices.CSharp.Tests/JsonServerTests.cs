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
	public class JsonServerTests
	{
		[Test]
		public void JsonServerRequiresServices()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonServer(null, null));
			Assert.Throws<ArgumentNullException>(() => new JsonServer(new StubServer(), null));
		}
	}
}
