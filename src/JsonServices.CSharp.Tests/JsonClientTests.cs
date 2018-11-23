using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class JsonClientTests
	{
		[Test]
		public void JsonClientRequiresClientInstance()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonClient(null));
		}
	}
}
