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
			Assert.IsFalse(Matches(b => b.Add("Some", "Value"), null));
		}

		[Test]
		public void TestStringValueMatches()
		{
			Assert.IsTrue(EventFilter.Matches(default(string), default(string)));
			Assert.IsTrue(EventFilter.Matches(string.Empty, default(string)));
			Assert.IsTrue(EventFilter.Matches(default(string), string.Empty));
			Assert.IsTrue(EventFilter.Matches(string.Empty, string.Empty));
			Assert.IsTrue(EventFilter.Matches("foo", "SomeFoo"));
			Assert.IsFalse(EventFilter.Matches("goo", "SomeFoo"));
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
		public void TestGuidMatches()
		{
			Assert.IsTrue(EventFilter.Matches(string.Empty, default(Guid)));
			Assert.IsTrue(EventFilter.Matches(default(string), Guid.NewGuid()));
			Assert.IsFalse(EventFilter.Matches("invalid", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsFalse(EventFilter.Matches("aea069ce44ef486884ec0817f589c69", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsFalse(EventFilter.Matches("aea069ce44ef486884ec0817f589c697", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsTrue(EventFilter.Matches("{AEA069CE-44EF-4868-84EC-0817F589C695}", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsTrue(EventFilter.Matches("AEA069CE-44EF-4868-84EC-0817F589C695", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsTrue(EventFilter.Matches("aea069ce-44ef-4868-84ec-0817f589c695", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsTrue(EventFilter.Matches("aea069ce44ef486884ec0817f589c695", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsTrue(EventFilter.Matches("AEA069CE44EF486884EC0817F589C695", new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
			Assert.IsTrue(EventFilter.Matches(new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}").ToString(), new Guid("{AEA069CE-44EF-4868-84EC-0817F589C695}")));
		}

		[Test]
		public void EventArgWithStringPropertyTests()
		{
			var arg = new EventArgWithStringProperty();
			var name = nameof(EventArgWithStringProperty.Name);
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "null"), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "Bozo"), arg));
			Assert.IsFalse(Matches(b => b.Add("Foo", "Bar"), arg));

			arg = new EventArgWithStringProperty { Name = "BGWJJILLIGKKK" };
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "Bozo"), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "BGWJJILLIGKKK"), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "jill"), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "jilll"), arg));
			Assert.IsFalse(Matches(b => b.Add("Bar", "Foo"), arg));
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
			var name = nameof(EventArgWithLongProperty.RecordID);
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "  "), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "123"), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "1,2,123,432"), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "1234,32"), arg));
		}

		[Test]
		public void EventArgWithDecimalPropertyTests()
		{
			var arg = new EventArgWithProperty<decimal> { Value = 123 };
			var name = nameof(EventArgWithProperty<decimal>.Value);
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "  "), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "123"), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "1,2,123,432"), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "1234,32"), arg));
		}

		[Test]
		public void EventArgWithBoolPropertyTests()
		{
			var arg = new EventArgWithProperty<bool> { Value = true };
			var name = nameof(EventArgWithProperty<bool>.Value);
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "  "), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "True"), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "TRUE"), arg));
			Assert.IsFalse(Matches(b => b.Add(name, "false"), arg));
		}

		[Test]
		public void EventArgWithGuidPropertyTests()
		{
			var arg = new EventArgWithProperty<Guid> { Value = Guid.NewGuid() };
			var name = nameof(EventArgWithProperty<bool>.Value);
			Assert.IsTrue(Matches(b => b.Add(name, null), arg));
			Assert.IsTrue(Matches(b => b.Add(name, "  "), arg));
			Assert.IsTrue(Matches(b => b.Add(name, arg.Value.ToString("N")), arg));
			Assert.IsTrue(Matches(b => b.Add(name, arg.Value.ToString("D")), arg));
			Assert.IsTrue(Matches(b => b.Add(name, arg.Value.ToString("B")), arg));
			Assert.IsTrue(Matches(b => b.Add(name, arg.Value.ToString("P")), arg));
			Assert.IsTrue(Matches(b => b.Add(name, arg.Value.ToString("X")), arg));
			Assert.IsTrue(Matches(b => b.Add(name, arg.Value.ToString()), arg));
		}
	}
}
