using System.Globalization;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Serialization.Newtonsoft;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public class SerializerTestsNewtonsoft : SerializerTestsBase
	{
		protected override ISerializer Serializer { get; } = new Serializer();
	}
}
