using System;

namespace JsonServices.Services
{
	public interface IServiceExecutor
	{
		object Execute(string name, IRequestContext context, object parameters);

		void RegisterHandler(string name, Func<IRequestContext, object, object> execute);
	}
}
