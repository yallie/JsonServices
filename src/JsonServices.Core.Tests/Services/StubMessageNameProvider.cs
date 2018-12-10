using JsonServices.Services;

namespace JsonServices.Tests.Services
{
	public class StubMessageNameProvider : IMessageNameProvider
	{
		public StubMessageNameProvider(string fakeName)
		{
			FakeName = fakeName;
		}

		private string FakeName { get; }

		public string TryGetMessageName(string messageId)
		{
			return FakeName;
		}
	}
}
