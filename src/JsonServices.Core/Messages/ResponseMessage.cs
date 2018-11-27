using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	[DataContract]
	public class ResponseMessage
	{
		[DataMember(Name = "jsonrpc", EmitDefaultValue = true)]
		public string Version => "2.0";

		[DataMember(Name = "result", EmitDefaultValue = false)]
		public object Result { get; set; }

		[DataMember(Name = "error", EmitDefaultValue = false)]
		public Error Error { get; set; }

		[DataMember(Name = "id", EmitDefaultValue = false)]
		public string Id { get; set; }

		public override string ToString() => $"<-- " +
			(Error != null ? $"Error: {Error.Message} ({Error.Code})" : $"Ok: {Result}") +
			(Id != null ? $" #{Id}" : string.Empty);
	}
}
