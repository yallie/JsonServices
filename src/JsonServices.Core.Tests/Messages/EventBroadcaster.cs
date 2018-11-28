using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	public class EventBroadcaster : IReturnVoid
	{
		public const string AfterStartupEventName = "IFoo.AfterStartup";

		public const string BeforeShutdownEventName = "IBar.BeforeShutdown";

		public string EventName { get; set; }
	}
}
