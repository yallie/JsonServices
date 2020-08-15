using JsonServices.Serialization;
using JsonServices.Serialization.SystemTextJson;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public class SerializerTestsSystemTextJson : SerializerTestsBase
	{
		protected override ISerializer Serializer { get; } = new Serializer();
	}
}
