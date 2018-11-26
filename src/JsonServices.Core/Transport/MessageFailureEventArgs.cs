using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Transport
{
	public class MessageFailureEventArgs : MessageEventArgs
	{
		public Exception Exception { get; set; }
	}
}
