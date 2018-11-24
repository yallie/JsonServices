using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Services
{
	public interface IMessageTypeLocator
	{
		Type GetRequestType(string name);

		Type GetResponseType(string name);
	}
}
