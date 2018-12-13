using System.Security.Principal;

namespace JsonServices.Auth.SecureRemotePassword
{
	public interface ISrpAccountRepository
	{
		ISrpAccount FindByName(string userName);

		IIdentity GetIdentity(ISrpAccount account);
	}
}
