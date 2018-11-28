using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	public class EventBroadcasterService
	{
		public EventBroadcasterService(JsonServer server)
		{
			Server = server;
		}

		private JsonServer Server { get; set; }

		public void Execute(EventBroadcaster request)
		{
			var eventArgs = EventArgs.Empty;
			switch (request.EventName)
			{
				case EventBroadcaster.AfterStartupEventName:
					eventArgs = new PropertyChangedEventArgs(nameof(EventBroadcaster));
					break;

				case EventBroadcaster.BeforeShutdownEventName:
					eventArgs = new CancelEventArgs(true);
					break;
			}

			Server.Broadcast(request.EventName, eventArgs);
		}
	}
}
