using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	public class ResponseMessage
	{
		public string Id { get; set; }

		public object Result { get; set; }

		public Error Error { get; set; }
	}
}
