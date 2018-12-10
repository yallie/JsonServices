using System;
using System.Collections.Generic;
using JsonServices.Messages;
using JsonServices.Transport;

namespace JsonServices.Services
{
	public interface IRequestContext : IDisposable
	{
		JsonServer Server { get; }

		string ConnectionId { get; }

		IConnection Connection { get; }

		RequestMessage RequestMessage { get; set; }

		IDictionary<string, object> Properties { get; }
	}
}
