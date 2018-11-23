using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	public class ResponseMessage<T> : IResponseMessage
	{
		public string Id { get; set; }

		public T Result { get; set; }

		public Error Error { get; set; }

		object IResponseMessage.Result => Result;
	}
}
