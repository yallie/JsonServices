using System;
using JsonServices.Messages;
using JsonServices.Serialization;
using JsonServices.Services;

namespace JsonServices.Tests.Serialization
{
	public class BrokenSerializer : ISerializer
	{
		public IMessage Deserialize(string data, IMessageTypeProvider typeProvider, IMessageNameProvider nameProvider) =>
			throw new NotImplementedException();

		public string Serialize(IMessage message) =>
			throw new NotImplementedException();
	}
}
