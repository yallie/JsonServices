using System;

namespace JsonServices.Transport
{
	public class MessageEventArgs : EventArgs
	{
		public string SessionId { get; set; }

		public string Data { get; set; }
	}
}