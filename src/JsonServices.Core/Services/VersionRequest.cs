using JsonServices.Messages;

namespace JsonServices.Services
{
	public sealed class VersionRequest : IReturn<VersionResponse>, ICustomName
	{
		public const string MessageName = "rpc.version";

		string ICustomName.MessageName => MessageName;
	}
}
