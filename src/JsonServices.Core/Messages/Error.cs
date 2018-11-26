using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	[DataContract]
	public class Error
	{
		[DataMember(Name = "code")]
		public int Code { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "data", EmitDefaultValue = false)]
		public string Data { get; set; }
	}
}
