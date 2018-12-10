using System;

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
