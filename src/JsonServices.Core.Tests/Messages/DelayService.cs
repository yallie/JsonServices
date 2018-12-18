using System;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	public class DelayService
	{
		public async Task Execute(DelayRequest req)
		{
			await Task.Delay(TimeSpan.FromMilliseconds(req.Milliseconds));
		}
	}
}
