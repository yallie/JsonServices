using JsonServices.Serialization;
using JsonServices.Serialization.ServiceStack;
using NUnit.Framework;

namespace JsonServices.Tests.Serialization
{
	[TestFixture]
	public class SerializerTestsServiceStack : SerializerTestsBase
	{
		protected override ISerializer Serializer { get; } = new Serializer();

		[Test, Ignore("ServiceStack doesn't support ValueTuple")]
		public override void SerializerCanSerializeRequestMessagesWithValueTuples()
		{
			base.SerializerCanSerializeRequestMessagesWithValueTuples();
		}

		[Test, Ignore("ServiceStack doesn't support ValueTuple")]

		public override void SerializerCanSerializeValueTuplesUpTo7TypeParameters()
		{
			base.SerializerCanSerializeValueTuplesUpTo7TypeParameters();
		}

		[Test, Ignore("ServiceStack can't deserialize anonymous types")]
		public override void SerializerCanHandleAnonymousTypes()
		{
			base.SerializerCanHandleAnonymousTypes();
		}
	}
}
