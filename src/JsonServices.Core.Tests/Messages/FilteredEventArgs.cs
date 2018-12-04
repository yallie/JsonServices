using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Tests.Messages
{
	[Serializable]
	public class FilteredEventArgs : EventArgs
	{
		public string StringProperty { get; set; }

		public bool BoolProperty { get; set; }

		public int IntProperty { get; set; }
	}
}
