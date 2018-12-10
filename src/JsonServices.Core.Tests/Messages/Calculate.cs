namespace JsonServices.Tests.Messages
{
	public class Calculate : IReturn<CalculateResponse>
	{
		public decimal FirstOperand { get; set; }

		public decimal SecondOperand { get; set; }

		public string Operation { get; set; }
	}
}
