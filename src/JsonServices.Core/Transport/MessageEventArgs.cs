using System;

namespace JsonServices.Transport
{
	public class MessageEventArgs : EventArgs
	{
		public string ConnectionId { get; set; }

		public string Data { get; set; }
	}
}