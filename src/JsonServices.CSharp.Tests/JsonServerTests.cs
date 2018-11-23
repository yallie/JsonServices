using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonServices.Tests
{
	[TestFixture]
	public class JsonServerTests
	{
		[Test]
		public void JsonServerRequiresClientInstance()
		{
			Assert.Throws<ArgumentNullException>(() => new JsonServer(null));
		}
	}
}
