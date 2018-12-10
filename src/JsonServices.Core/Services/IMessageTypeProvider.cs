using System;

namespace JsonServices.Services
{
	public interface IMessageTypeProvider
	{
		void Register(string name, Type requestType, Type responseType = null);

		Type TryGetRequestType(string name);

		Type TryGetResponseType(string name);
	}
}
