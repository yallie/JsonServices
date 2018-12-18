namespace JsonServices.Tests.Messages
{
	public class DelayRequest : IReturnVoid
	{
		public int Milliseconds { get; set; }
	}
}
