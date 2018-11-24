using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubExecutor : IServiceExecutor
	{
		public object Execute(string name, object parameters)
		{
			if (name == typeof(GetVersion).FullName)
			{
				var service = new GetVersionService();
				return service.Execute((GetVersion)parameters);
			}

			if (name == typeof(Calculate).FullName)
			{
				var service = new CalculateService();
				return service.Execute((Calculate)parameters);
			}

			throw new MethodNotFoundException(name);
		}
	}
}
