using System;

namespace JsonServices.Services
{
	public interface IServiceExecutor
	{
		object Execute(string name, RequestContext context, object parameters);

		void RegisterHandler(string name, Func<RequestContext, object, object> execute);
	}
}
