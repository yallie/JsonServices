namespace JsonServices.Tests.Messages
{
	public class EventBroadcaster : IReturnVoid
	{
		public const string AfterStartupEventName = "IFoo.AfterStartup";

		public const string BeforeShutdownEventName = "IBar.BeforeShutdown";

		public const string FilteredEventName = "FilteredEvent";

		public string EventName { get; set; }

		public string StringArgument { get; set; }

		public bool BoolArgument { get; set; }

		public int IntArgument { get; set; }
	}
}
