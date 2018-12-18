using System;
using JsonServices.Tests.Services;

namespace JsonServices.Tests.Messages.Generic
{
	public class GenericMessageTypeProvider : StubMessageTypeProvider
	{
		private Type GetValueType(string valueType)
		{
			switch ($"{valueType}".ToLower())
			{
				case "integer":
					return typeof(int);

				case "string":
					return typeof(string);

				case "date":
					return typeof(DateTime);
			}

			return null;
		}

		private Type GetMessageType(string requestName, bool isRequest)
		{
			var prefix = typeof(GenericRequest<>).FullName + ":";
			if (!requestName.StartsWith(prefix))
			{
				return null;
			}

			var valueType = requestName.Substring(prefix.Length);
			var typeParameter = GetValueType(valueType);
			if (typeParameter == null)
			{
				return null;
			}

			var baseType = isRequest ? typeof(GenericRequest<>) : typeof(GenericResponse<>);
			return baseType.MakeGenericType(typeParameter);
		}

		public override Type TryGetRequestType(string name) =>
			base.TryGetRequestType(name) ?? GetMessageType(name, isRequest: true);

		public override Type TryGetResponseType(string name) =>
			base.TryGetResponseType(name) ?? GetMessageType(name, isRequest: false);
	}
}
