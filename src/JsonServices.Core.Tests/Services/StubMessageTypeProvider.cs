using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubMessageTypeProvider : MessageTypeProvider
	{
		public StubMessageTypeProvider()
		{
			Register(typeof(Calculate).FullName, typeof(Calculate));
			Register(typeof(DelayRequest).FullName, typeof(DelayRequest));
			Register(typeof(GetVersion).FullName, typeof(GetVersion));
			Register(typeof(EventBroadcaster).FullName, typeof(EventBroadcaster));
		}
	}
}
