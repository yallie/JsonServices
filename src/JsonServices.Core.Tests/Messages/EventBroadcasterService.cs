using System;
using System.ComponentModel;

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
					eventArgs = new MyCoolEventArgs
					{
						PropertyName = nameof(EventBroadcaster)
					};
					break;

				case EventBroadcaster.BeforeShutdownEventName:
					eventArgs = new CancelEventArgs(true);
					break;

				case EventBroadcaster.FilteredEventName:
					eventArgs = new FilteredEventArgs
					{
						StringProperty = request.StringArgument,
						IntProperty = request.IntArgument,
						BoolProperty = request.BoolArgument,
					};
					break;
			}

			Server.Broadcast(request.EventName, eventArgs);
		}
	}
}
