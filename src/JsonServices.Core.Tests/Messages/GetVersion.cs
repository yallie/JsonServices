namespace JsonServices.Tests.Messages
{
	public class GetVersion : IReturn<GetVersionResponse>
	{
		public bool IsInternal { get; set; }
	}
}
