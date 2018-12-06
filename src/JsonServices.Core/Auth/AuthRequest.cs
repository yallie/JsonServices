using System.Collections.Generic;
using JsonServices.Messages;

namespace JsonServices.Auth
{
	public class AuthRequest : IReturn<AuthResponse>, ICustomName
	{
		public const string MessageName = "rpc.authenticate";

		string ICustomName.MessageName => MessageName;

		public const string UserNameKey = "UserName";

		public const string PasswordKey = "Password";

		public Dictionary<string, string> Parameters { get; set; } =
			new Dictionary<string, string>();
	}
}
