using JsonServices.Serialization;
using JsonServices.Serialization.SystemTextJson;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture, Ignore("Doesn't work yet")]
	public class SerializerTestsSystemTextJson : SerializerTestsBase
	{
		protected override ISerializer Serializer { get; } = new Serializer();
	}
}
