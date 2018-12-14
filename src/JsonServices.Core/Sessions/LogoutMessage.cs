using JsonServices.Messages;

namespace JsonServices.Sessions
{
	public class LogoutMessage : ICustomName, IReturnVoid
	{
		public const string MessageName = "rpc.logout";

		string ICustomName.MessageName => MessageName;
	}
}
