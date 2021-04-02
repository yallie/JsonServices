using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JsonServices.Auth
{
	public class JsonIdentity : IIdentity, IEquatable<JsonIdentity>
	{
		public JsonIdentity()
		{
		}

		public JsonIdentity(string name, string type)
		{
			Name = name;
			AuthenticationType = type;
		}

		public JsonIdentity(IIdentity identity) =>
			CopyFrom(identity);

		private void CopyFrom(IIdentity identity)
		{
			Name = identity.Name;
			AuthenticationType = identity.AuthenticationType;
			if (identity is JsonIdentity ji)
			{
				Properties = ji.Properties.ToDictionary(p => p.Key, p => p.Value);
			}
		}

		public bool Equals(JsonIdentity other)
		{
			if (other == null)
			{
				return false;
			}

			if (Name != other.Name || AuthenticationType != other.AuthenticationType)
			{
				return false;
			}

			if (Properties.Count != other.Properties.Count)
			{
				return false;
			}

			foreach (var p in Properties)
			{
				if (!other.Properties.TryGetValue(p.Key, out var value))
				{
					return false;
				}

				if (value != p.Value)
				{
					return false;
				}
			}

			return true;
		}

		public override bool Equals(object obj) => Equals(obj as JsonIdentity);

		public override int GetHashCode() => Name.GetHashCode();

		public string Name { get; set; }

		public string AuthenticationType { get; set; }

		public virtual bool IsAuthenticated => !string.IsNullOrEmpty(AuthenticationType);

		public Dictionary<string, string> Properties { get; private set; } =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}
}
