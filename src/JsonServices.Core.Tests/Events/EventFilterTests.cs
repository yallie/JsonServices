using System;
using System.Collections.Generic;
using JsonServices.Events;
using NUnit.Framework;

namespace JsonServices.Tests.Events
{
	[TestFixture]
	public class EventFilterTests
	{
		public class Builder
		{
			public Dictionary<string, string> Filter { get; } =
				new Dictionary<string, string>();

			public Builder Add(string propertyName, string value)
			{
				Filter[propertyName] = value;
				return this;
			}
		}

		private bool Matches(Func<Builder, Builder> init, EventArgs args)
		{
			return EventFilter.Matches(init(new Builder()).Filter, args);
		}

		[Test]
		public void EmptyEventFilterMatchesAnything()
		{
			Assert.IsTrue(EventFilter.Matches<EventArgs>(null, null));
			Assert.IsTrue(EventFilter.Matches(null, EventArgs.Empty));
			Assert.IsTrue(EventFilter.Matches(new Dictionary<string, string>(), EventArgs.Empty));
		}

		[Test]
		public void EventArgWithStringPropertyTests()
		{
			var arg = new EventArgWithStringProperty();
			var name = nameof(EventArgWithStringProperty.Name);
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "null"), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "Bozo"), arg));

			arg = new EventArgWithStringProperty { Name = "BGWJJILLIGKKK" };
			Assert.IsFalse(Matches(b => b.Add(name, null), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "BGWJJILLIGKKK"), arg));
		}

		[Test]
		public void EventArgWithLongPropertyTests()
		{
			var arg = new EventArgWithLongProperty { RecordID = 123 };
			Assert.IsTrue(Matches(b => b.Add("RecordID", "123"), arg));
		}
	}
}
