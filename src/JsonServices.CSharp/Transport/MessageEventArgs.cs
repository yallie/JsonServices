using System;

namespace JsonServices.Transport
{
	public class MessageEventArgs
	{
		public Guid SessionId { get; set; }

		public byte[] Data { get; set; }
	}
}