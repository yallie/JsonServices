using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Serialization;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Serialization
{
	public class StubLocator : IMessageTypeLocator
	{
		public Type GetRequestType(string name)
		{
			switch (name)
			{
				case "JsonServices.Tests.Messages.GetVersion":
					return typeof(GetVersion);

				default:
					throw new InvalidOperationException($"Message request type not found: {name}");
			}
		}

		public Type GetResponseType(string name)
		{
			var type = GetRequestType(name);
			var retTypes =
				from inter in type.GetTypeInfo().GetInterfaces()
				where inter.FullName.StartsWith("JsonServices.IReturn")
				select inter;

			var retType = retTypes.Single();
			if (retType == typeof(IReturnVoid))
			{
				return typeof(void);
			}

			if (retType.IsGenericType)
			{
				return retType.GetGenericArguments().Single();
			}

			throw new InvalidOperationException($"Message response type not found: {name}");
		}
	}
}
