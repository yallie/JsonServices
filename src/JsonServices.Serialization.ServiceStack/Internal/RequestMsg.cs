using System.Runtime.Serialization;

namespace JsonServices.Serialization.ServiceStack.Internal
{
	[DataContract]
	internal class RequestMsg<T> : IRequestMessage
	{
		[DataMember(Name = "params")]
		public T Parameters { get; set; }
		object IRequestMessage.Parameters => Parameters;
	}
}
