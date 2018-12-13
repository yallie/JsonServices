using System;

namespace JsonServices.Tests.Events
{
	public class EventArgWithProperty<T> : EventArgs
	{
		public T Value { get; set; }
	}
}
