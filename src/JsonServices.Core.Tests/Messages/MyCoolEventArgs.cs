using System;

namespace JsonServices.Tests.Messages
{
	[Serializable]
	public class MyCoolEventArgs : EventArgs
	{
		public string PropertyName { get; set; }
	}
}
