using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	internal class GetVersionService : IService<GetVersion>
	{
		public object Execute(GetVersion request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			if (request.IsInternal)
			{
				return "Version 0.01-alpha, build 12345, by yallie";
			}

			return "0.01-alpha";
		}
	}
}
