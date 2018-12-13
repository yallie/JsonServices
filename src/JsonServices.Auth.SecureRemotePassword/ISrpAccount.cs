namespace JsonServices.Auth.SecureRemotePassword
{
	public interface ISrpAccount
	{
		string UserName { get; }

		string Salt { get; }

		string Verifier { get; }
	}
}
