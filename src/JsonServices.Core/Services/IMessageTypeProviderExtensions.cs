using System;
using JsonServices.Exceptions;

namespace JsonServices.Services
{
	public static class IMessageTypeProviderExtensions
	{
		public static Type GetRequestType(this IMessageTypeProvider self, string name)
		{
			var result = self.TryGetRequestType(name);
			if (result != null)
			{
				return result;
			}

			throw new MethodNotFoundException(name);
		}

		public static Type GetResponseType(this IMessageTypeProvider self, string name)
		{
			var result = self.TryGetResponseType(name);
			if (result != null)
			{
				return result;
			}

			throw new MethodNotFoundException(name);
		}
	}
}
