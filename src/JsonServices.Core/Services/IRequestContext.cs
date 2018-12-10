using System;
using System.Collections.Generic;
using JsonServices.Transport;

namespace JsonServices.Services
{
	public interface IRequestContext : IDisposable
	{
		JsonServer Server { get; }

		string ConnectionId { get; }

		IConnection Connection { get; }

		IDictionary<string, object> Properties { get; }
	}
}
