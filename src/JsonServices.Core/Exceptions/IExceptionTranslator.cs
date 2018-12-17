using System;
using JsonServices.Messages;

namespace JsonServices.Exceptions
{
	public interface IExceptionTranslator
	{
		Error Translate(Exception ex, int? code = null, string message = null);
	}
}
