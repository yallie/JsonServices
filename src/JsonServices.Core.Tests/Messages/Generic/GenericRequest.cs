using JsonServices.Messages;

namespace JsonServices.Tests.Messages.Generic
{
	public class GenericRequest<T> : ICustomName, IGenericRequest, IReturn<GenericResponse<T>>
	{
		public string MessageName => $"{typeof(GenericRequest<>).FullName}:{ValueType}";

		public string ValueType
		{
			get
			{
				switch (typeof(T).Name)
				{
					case "Int32": return "Integer";
					case "DateTime": return "Date";
					default: return typeof(T).Name;
				}
			}
		}

		public T Value { get; set; }

		object IGenericRequest.Value => Value;

		public object CreateResponse(object result) => new GenericResponse<T>
		{
			Result = (T)result,
		};
	}
}
