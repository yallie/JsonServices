using System;

namespace JsonServices.Transport
{
	public interface IConnection
	{
		string ConnectionId { get; }
	}
}