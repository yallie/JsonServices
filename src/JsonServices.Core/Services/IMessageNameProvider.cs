using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Services
{
	public interface IMessageNameProvider
	{
		string GetMessageName(string messageId);
	}
}
