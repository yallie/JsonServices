using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;

namespace JsonServices.Services
{
	public class MessageTypeProvider : IMessageTypeProvider
	{
		private ConcurrentDictionary<string, Type> RequestTypes { get; } =
			new ConcurrentDictionary<string, Type>();

		private ConcurrentDictionary<string, Type> ResponseTypes { get; } =
			new ConcurrentDictionary<string, Type>();

		public Type GetRequestType(string name)
		{
			var result = TryGetRequestType(name);
			if (result != null)
			{
				return result;
			}

			throw new MethodNotFoundException(name);
		}

		public Type GetResponseType(string name)
		{
			var result = TryGetResponseType(name);
			if (result != null)
			{
				return result;
			}

			throw new MethodNotFoundException(name);
		}

		public void Register(string name, Type requestType, Type responseType = null)
		{
			RequestTypes[name] = requestType ?? throw new ArgumentNullException(nameof(requestType));
			ResponseTypes[name] = responseType ?? TryGetResponseType(requestType) ?? throw new ArgumentNullException(nameof(responseType));
		}

		protected virtual Type TryGetRequestType(string name)
		{
			if (RequestTypes.TryGetValue(name, out var result))
			{
				return result;
			}

			return null;
		}

		protected virtual Type TryGetResponseType(string name)
		{
			if (ResponseTypes.TryGetValue(name, out var result))
			{
				return result;
			}

			return null;
		}

		public virtual string IReturnVoidInterfaceName { get; set; } = "IReturnVoid";

		public virtual string IReturnInterfaceName { get; set; } = "IReturn`1";

		protected virtual Type TryGetResponseType(Type requestType)
		{
			// support JsonService.IReturn and ServiceStack.IReturn
			var retTypes =
				from inter in requestType.GetTypeInfo().GetInterfaces()
				where
					(inter.Name == IReturnVoidInterfaceName && !inter.IsGenericType) ||
					(inter.Name == IReturnInterfaceName && inter.IsGenericType)
				select inter;

			var retType = retTypes.FirstOrDefault();
			if (retType == null)
			{
				return null;
			}

			if (retType.Name == IReturnVoidInterfaceName)
			{
				return typeof(void);
			}

			if (retType.IsGenericType)
			{
				return retType.GetGenericArguments().Single();
			}

			return null;
		}
	}
}
