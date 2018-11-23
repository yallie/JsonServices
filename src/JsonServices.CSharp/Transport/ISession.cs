using System;

namespace JsonServices.Transport
{
	public interface ISession
	{
		Guid SessionId { get; }
	}
}