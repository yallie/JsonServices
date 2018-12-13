using System.Security.Principal;

namespace JsonServices.Auth
{
	public class AnonymousIdentity : IIdentity
	{
		public string Name => "Anonymous";

		public string AuthenticationType => "None";

		public bool IsAuthenticated => false;
	}
}
