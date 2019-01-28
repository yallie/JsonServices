using System;
using JsonServices.Services;

namespace JsonServices.Tests.Services
{
	public class BrokenMessageTypeProvider : IMessageTypeProvider
	{
		public void Register(string name, Type requestType, Type responseType = null) =>
			throw new NotImplementedException();

		public Type TryGetRequestType(string name) =>
			throw new NotImplementedException();

		public Type TryGetResponseType(string name) =>
			throw new NotImplementedException();
	}
}
