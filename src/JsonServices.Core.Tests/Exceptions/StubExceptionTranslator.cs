using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Messages;

namespace JsonServices.Tests.Exceptions
{
	public class StubExceptionTranslator : ExceptionTranslator
	{
		public override Error Translate(Exception ex, int? code = null, string message = null)
		{
			var result = base.Translate(ex, code, message);
			ErrorTranslated?.Invoke(this, result);
			return result;
		}

		public event EventHandler<Error> ErrorTranslated;
	}
}
