using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Serialization.ServiceStack;
using JsonServices.Tests.Messages;
using JsonServices.Tests.Services;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public class SerializerTestsServiceStack : SerializerTestsBase
	{
		protected override ISerializer Serializer { get; } =
			new Serializer(new StubMessageTypeProvider(), new StubMessageNameProvider(typeof(GetVersion).FullName));

		[Test]
		public void RequiredAndOptionalArguments()
		{
			Assert.Throws<ArgumentNullException>(() => new Serializer(null, null));
			Assert.DoesNotThrow(() => new Serializer(new StubMessageTypeProvider(), null));
		}
	}
}
