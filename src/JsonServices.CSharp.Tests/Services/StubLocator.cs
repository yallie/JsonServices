using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubLocator : IMessageTypeLocator
	{
		private Dictionary<string, Type> RequestTypes { get; } =
			new Dictionary<string, Type>
			{
				{ typeof(GetVersion).FullName, typeof(GetVersion) },
				{ typeof(Calculate).FullName, typeof(Calculate) },
			};

		public Type GetRequestType(string name)
		{
			if (RequestTypes.TryGetValue(name, out var result))
			{
				return result;
			}

			throw new MethodNotFoundException(name);
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

			throw new MethodNotFoundException(name);
		}
	}
}
