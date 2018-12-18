using System;
using System.Threading.Tasks;
using JsonServices.Tests.Services;

namespace JsonServices.Tests.Messages.Generic
{
	public class GenericServiceExecutor : StubExecutor
	{
		public override object Execute(string name, object parameters)
		{
			// handle generic messages
			var prefix = typeof(GenericRequest<>).FullName;
			if (name.StartsWith(prefix))
			{
				return ExecuteGenericRequest(parameters as IGenericRequest);
			}

			// handle other messages
			return base.Execute(name, parameters);
		}

		private async Task<object> ExecuteGenericRequest(IGenericRequest request)
		{
			if (request.Value is int num)
			{
				await Task.Delay(1);
				return request.CreateResponse(num + 1);
			}

			if (request.Value is DateTime dt)
			{
				await Task.Delay(2);
				return request.CreateResponse(dt.AddYears(1));
			}

			if (request.Value is string s)
			{
				await Task.Delay(3);
				return request.CreateResponse($"Hello {s}!");
			}

			return request.CreateResponse(null);
		}
	}
}
