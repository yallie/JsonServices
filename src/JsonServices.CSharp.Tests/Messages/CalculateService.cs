using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	public class CalculateService
	{
		public CalculateResponse Execute(Calculate request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			var result = default(decimal);
			switch (request.Operation)
			{
				case "+":
					result = request.FirstOperand + request.SecondOperand;
					break;

				case "-":
					result = request.FirstOperand - request.SecondOperand;
					break;

				case "*":
					result = request.FirstOperand * request.SecondOperand;
					break;

				case "/":
					result = request.FirstOperand / request.SecondOperand;
					break;

				case "%":
					result = request.FirstOperand % request.SecondOperand;
					break;

				default:
					throw new InvalidOperationException($"Bad operation: {request.Operation}");
			}

			return new CalculateResponse
			{
				Result = result
			};
		}
	}
}
