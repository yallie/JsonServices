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

		public string GetMessageName(string messageId)
		{
			return FakeName;
		}
	}
}
