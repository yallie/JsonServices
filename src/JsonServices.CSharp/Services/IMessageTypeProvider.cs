using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Services
{
	public interface IMessageTypeProvider
	{
		void Register(string name, Type requestType, Type responseType = null);

		Type GetRequestType(string name);

		Type GetResponseType(string name);
	}
}
