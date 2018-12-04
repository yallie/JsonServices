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
		public void TestStringValueMatches()
		{
			Assert.IsTrue(EventFilter.Matches(default(string), default(string)));
			Assert.IsTrue(EventFilter.Matches(string.Empty, default(string)));
			Assert.IsTrue(EventFilter.Matches(default(string), string.Empty));
			Assert.IsTrue(EventFilter.Matches(string.Empty, string.Empty));
			Assert.IsTrue(EventFilter.Matches("foo", "SomeFoo"));
		}

		[Test]
		public void TestDecimalValueMatches()
		{
			Assert.IsTrue(EventFilter.Matches(default(string), 1));
			Assert.IsTrue(EventFilter.Matches(string.Empty, 123));
			Assert.IsTrue(EventFilter.Matches("123", 123));
			Assert.IsFalse(EventFilter.Matches("12345", 123));
			Assert.IsFalse(EventFilter.Matches("12345", 345));
			Assert.IsTrue(EventFilter.Matches("123,45", 123));
			Assert.IsTrue(EventFilter.Matches("123,45.67", 45.67m));
		}

		[Test]
		public void TestBooleanValueMatches()
		{
			Assert.IsTrue(EventFilter.Matches(default(string), true));
			Assert.IsTrue(EventFilter.Matches(string.Empty, false));
			Assert.IsTrue(EventFilter.Matches("True", true));
			Assert.IsTrue(EventFilter.Matches("true", true));
			Assert.IsFalse(EventFilter.Matches("True", false));
			Assert.IsFalse(EventFilter.Matches("true", false));
			Assert.IsTrue(EventFilter.Matches("FALSE", false));
			Assert.IsTrue(EventFilter.Matches("false", false));
			Assert.IsFalse(EventFilter.Matches("FALSE", true));
			Assert.IsFalse(EventFilter.Matches("false", true));
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
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "Bozo"), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "BGWJJILLIGKKK"), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "jill"), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "jilll"), arg));
		}

		[Test]
		public void EventArgWithIntPropertyTests()
		{
			var args = new EventArgWithIntProperty { Count = 10 };
			var name = nameof(EventArgWithIntProperty.Count);
			Assert.IsTrue(Matches(b => b.Add(name, "10"), args));
			Assert.IsFalse(Matches(b => b.Add(name, "12"), args));
			Assert.IsFalse(Matches(b => b.Add(name, "100"), args));
			Assert.IsTrue(Matches(b => b.Add(name, "8,9,10,11"), args));
		}

		[Test]
		public void EventArgWithLongPropertyTests()
		{
			var arg = new EventArgWithLongProperty { RecordID = 123 };
			Assert.IsTrue(Matches(b => b.Add("RecordID", null), arg));
			Assert.IsTrue(Matches(b => b.Add("RecordID", "  "), arg));
			Assert.IsTrue(Matches(b => b.Add("RecordID", "123"), arg));
			Assert.IsTrue(Matches(b => b.Add("RecordID", "1,2,123,432"), arg));
			Assert.IsFalse(Matches(b => b.Add("RecordID", "1234,32"), arg));
		}
	}
}
