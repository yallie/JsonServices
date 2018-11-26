using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	public class Calculate : IReturn<CalculateResponse>
	{
		public decimal FirstOperand { get; set; }

		public decimal SecondOperand { get; set; }

		public string Operation { get; set; }
	}
}
