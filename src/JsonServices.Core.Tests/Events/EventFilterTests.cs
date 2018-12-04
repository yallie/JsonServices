using System;
using System.Collections.Generic;
using JsonServices.Events;
using NUnit.Framework;

namespace JsonServices.Tests.Events
{
	[TestFixture]
	public class EventFilterTests
	{
		[Test]
		public void EmptyEventFilterMatchesAnything()
		{
			Assert.IsTrue(EventFilter.Matches<EventArgs>(null, null));
			Assert.IsTrue(EventFilter.Matches(null, EventArgs.Empty));
			Assert.IsTrue(EventFilter.Matches(new Dictionary<string, string>(), EventArgs.Empty));
		}
	}
}
