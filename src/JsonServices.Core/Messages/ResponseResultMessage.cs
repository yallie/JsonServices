using System.Runtime.Serialization;

namespace JsonServices.Messages
{
	[DataContract]
	public class ResponseResultMessage : ResponseMessage
	{
		[DataMember(Name = "jsonrpc", EmitDefaultValue = true)]
		public override string Version => "2.0";

		[DataMember(Name = "result", EmitDefaultValue = true)]
		public override object Result { get; set; }

		[IgnoreDataMember]
		public override Error Error { get; set; }
	}
}
