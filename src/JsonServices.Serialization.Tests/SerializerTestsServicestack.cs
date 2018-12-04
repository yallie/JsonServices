using JsonServices.Serialization;
using JsonServices.Serialization.ServiceStack;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public class SerializerTestsServiceStack : SerializerTestsBase
	{
		protected override ISerializer Serializer { get; } = new Serializer();
	}
}
