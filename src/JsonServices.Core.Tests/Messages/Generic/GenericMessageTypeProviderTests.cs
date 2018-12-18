using System;
using NUnit.Framework;

namespace JsonServices.Tests.Messages.Generic
{
	[TestFixture]
	public class GenericMessageTypeProviderTests
	{
		private GenericMessageTypeProvider Provider { get; } = new GenericMessageTypeProvider();

		[Test]
		public void TypeProviderCreatesProperRequestTypes()
		{
			var baseName = typeof(GenericRequest<>).FullName;
			Assert.AreEqual(typeof(GenericRequest<int>), Provider.TryGetRequestType($"{baseName}:Integer"));
			Assert.AreEqual(typeof(GenericRequest<string>), Provider.TryGetRequestType($"{baseName}:STRING"));
			Assert.AreEqual(typeof(GenericRequest<DateTime>), Provider.TryGetRequestType($"{baseName}:date"));
			Assert.IsNull(Provider.TryGetRequestType($"{baseName}:foo"));
		}

		[Test]
		public void TypeProviderCreatesProperResponseTypes()
		{
			var baseName = typeof(GenericRequest<>).FullName;
			Assert.AreEqual(typeof(GenericResponse<int>), Provider.TryGetResponseType($"{baseName}:INTEGER"));
			Assert.AreEqual(typeof(GenericResponse<string>), Provider.TryGetResponseType($"{baseName}:string"));
			Assert.AreEqual(typeof(GenericResponse<DateTime>), Provider.TryGetResponseType($"{baseName}:Date"));
			Assert.IsNull(Provider.TryGetResponseType($"{baseName}:foo"));
		}
	}
}
