using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Messages
{
	public class RequestMessage
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public object Params { get; set; }
	}
}
