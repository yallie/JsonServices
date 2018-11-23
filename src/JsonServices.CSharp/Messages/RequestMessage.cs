using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	public class RequestMessage<T> : IRequestMessage
	{
		public RequestMessage()
		{
			Name = typeof(T).FullName;
		}

		public string Id { get; set; }

		public bool IsOneWay => string.IsNullOrWhiteSpace(Id);

		public string Name { get; set; }

		public T Params { get; set; }

		object IRequestMessage.Params => Params;
	}
}
