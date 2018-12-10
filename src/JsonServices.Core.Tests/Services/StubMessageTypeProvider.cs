using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubMessageTypeProvider : MessageTypeProvider
	{
		public StubMessageTypeProvider()
		{
			Register(typeof(GetVersion).FullName, typeof(GetVersion));
			Register(typeof(Calculate).FullName, typeof(Calculate));
			Register(typeof(EventBroadcaster).FullName, typeof(EventBroadcaster));
		}
	}
}
