namespace JsonServices.Services
{
	public interface IMessageNameProvider
	{
		string TryGetMessageName(string messageId);
	}
}
