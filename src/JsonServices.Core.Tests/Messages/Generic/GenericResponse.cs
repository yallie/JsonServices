namespace JsonServices.Tests.Messages.Generic
{
	public class GenericResponse<T>
	{
		// T is determined by GenericRequest.ValueType property
		public T Result { get; set; }
	}
}
