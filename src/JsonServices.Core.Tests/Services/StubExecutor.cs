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
			RegisterHandler(typeof(GetVersion).FullName, (s, p) =>
			{
				var service = new GetVersionService();
				return service.Execute((GetVersion)p);
			});

			RegisterHandler(typeof(Calculate).FullName, (s, p) =>
			{
				var service = new CalculateService();
				return service.Execute((Calculate)p);
			});

			RegisterHandler(typeof(EventBroadcaster).FullName, (s, p) =>
			{
				var service = new EventBroadcasterService(s.Server);
				service.Execute((EventBroadcaster)p);
				return null;
			});
		}
	}
}
