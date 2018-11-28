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
	public class StubExecutor : ServiceExecutor
	{
		public StubExecutor()
		{
			RegisterHandler(typeof(GetVersion).FullName, parameters =>
			{
				var service = new GetVersionService();
				return service.Execute((GetVersion)parameters);
			});

			RegisterHandler(typeof(Calculate).FullName, parameters =>
			{
				var service = new CalculateService();
				return service.Execute((Calculate)parameters);
			});
		}
	}
}
