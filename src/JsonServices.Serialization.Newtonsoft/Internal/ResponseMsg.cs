using System.Runtime.Serialization;

namespace JsonServices.Serialization.Newtonsoft.Internal
{
	[DataContract]
	internal class ResponseMsg<T> : IResponseMessage
	{
		[DataMember(Name = "result")]
		public T Result { get; set; }
		object IResponseMessage.Result => Result;
	}
}
