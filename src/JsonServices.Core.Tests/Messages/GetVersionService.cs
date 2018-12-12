using System;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	internal class GetVersionService
	{
		public async Task<GetVersionResponse> Execute(GetVersion request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			if (request.IsInternal)
			{
				// simulate long-running operation
				await Task.Delay(3);

				return new GetVersionResponse
				{
					Version = "Version 0.01-alpha, build 12345, by yallie"
				};
			}

			return new GetVersionResponse
			{
				Version = "0.01-alpha"
			};
		}

		public GetVersionResponse Execute(IReturn<GetVersionResponse> request)
		{
			throw new NotImplementedException();
		}
	}
}
