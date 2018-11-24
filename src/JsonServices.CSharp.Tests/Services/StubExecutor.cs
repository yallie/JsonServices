using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubExecutor : IServiceExecutor
	{
		public object Execute(string name, object parameters)
		{
			if (name == "JsonServices.Tests.Messages.GetVersion")
			{
				var service = new GetVersionService();
				return service.Execute((GetVersion)parameters);
			}

			throw new InvalidOperationException($"Unregistered service: {name}");
		}
	}
}
