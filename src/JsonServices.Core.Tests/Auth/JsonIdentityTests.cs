using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using JsonServices.Auth;
using NUnit.Framework;

namespace JsonServices.Tests.Auth
{
	[TestFixture]
	public class JsonIdentityTests
	{
		private class AnonIdentity : IIdentity
		{
			public string Name => "Anonymous";

			public string AuthenticationType => "None";

			public bool IsAuthenticated => false;
		}

		[Test]
		public void JsonIdentityBasedOnIIdentity()
		{
			var anon = new AnonIdentity();
			var identity = new JsonIdentity(anon);
			Assert.AreEqual(anon.Name, identity.Name);
			Assert.AreEqual(anon.AuthenticationType, identity.AuthenticationType);
		}

		[Test]
		public void JsonIdentityBasedOnJsonIdentity()
		{
			var identity = new JsonIdentity("Hello", "World");
			identity.Properties["Date"] = DateTime.Now.ToString();

			var copy = new JsonIdentity(identity);
			Assert.AreEqual(identity, copy);
		}

		[Test]
		public void JsonIdentityEqualityTests()
		{
			Assert.That(new JsonIdentity(), Is.Not.EqualTo(null));
			Assert.That(new JsonIdentity(), Is.EqualTo(new JsonIdentity()));
			Assert.That(new JsonIdentity("Hello", "World"), Is.EqualTo(new JsonIdentity("Hello", "World")));
			Assert.That(new JsonIdentity("Hello", "World"), Is.Not.EqualTo(new JsonIdentity("Goodbye", "World")));

			var a = new JsonIdentity("A", "B");
			a.Properties["C"] = "D";
			var b = new JsonIdentity("A", "B");
			b.Properties["C"] = "D";
			Assert.AreEqual(a, b);

			b.Properties["D"] = "E";
			Assert.AreNotEqual(a, b);
		}
	}
}
