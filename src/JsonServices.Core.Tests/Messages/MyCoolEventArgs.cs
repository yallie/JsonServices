using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	[Serializable]
	public class MyCoolEventArgs : EventArgs
	{
		public string PropertyName { get; set; }
	}
}
