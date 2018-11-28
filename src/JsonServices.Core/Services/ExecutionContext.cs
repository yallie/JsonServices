using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Transport;

namespace JsonServices.Services
{
	public class ExecutionContext
	{
		public JsonServer Server { get; set; }

		public string ConnectionId { get; set; }
	}
}
