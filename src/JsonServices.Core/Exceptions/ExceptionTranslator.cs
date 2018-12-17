using System;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	public class ExceptionTranslator : IExceptionTranslator
	{
		public virtual Error Translate(Exception ex, int? code = null, string message = null)
		{
			// don't translate anything by default
			var result = new Error(ex);

			// set default error code if not specified
			if (code.HasValue)
			{
				result.Code = code.Value;
			}
			else if (result.Code == 0)
			{
				result.Code = InternalErrorException.ErrorCode;
			}

			// override error message if needed
			if (message != null)
			{
				result.Message = message;
			}

			return result;
		}
	}
}
