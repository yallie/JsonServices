using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Exceptions;
using JsonServices.Services;
using JsonServices.Tests.Messages;

namespace JsonServices.Tests.Services
{
	public class StubLocator : MessageTypeLocator
	{
		public StubLocator()
		{
			Register(typeof(GetVersion).FullName, typeof(GetVersion));
			Register(typeof(Calculate).FullName, typeof(Calculate));
		}
	}
}
