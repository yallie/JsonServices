using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	[DataContract]
	public class RequestMessage
	{
		[DataMember(Name = "jsonrpc", EmitDefaultValue = true)]
		public string Version => "2.0";

		[DataMember(Name = "method", EmitDefaultValue = true)]
		public string Name { get; set; }

		[DataMember(Name = "params", EmitDefaultValue = true)]
		public object Parameters { get; set; }

		[DataMember(Name = "id", EmitDefaultValue = false)]
		public string Id { get; set; }

		[IgnoreDataMember]
		public bool IsOneWay => string.IsNullOrWhiteSpace(Id);
	}
}
