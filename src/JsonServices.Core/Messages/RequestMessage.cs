using System;
using System.Runtime.Serialization;

namespace JsonServices.Messages
{
	[DataContract]
	public class RequestMessage : IMessage, IEquatable<RequestMessage>
	{
		[DataMember(Name = "jsonrpc", EmitDefaultValue = true)]
		public string Version => "2.0";

		[DataMember(Name = "method", EmitDefaultValue = true)]
		public string Name { get; set; }

		[DataMember(Name = "params", EmitDefaultValue = true)]
		public object Parameters { get; set; }

		[DataMember(Name = "id", EmitDefaultValue = false)]
		public string Id { get; set; }

		[IgnoreDataMember]
		public bool IsNotification => string.IsNullOrWhiteSpace(Id);

		public override string ToString() => $"--> {Name}" +
			(Id != null ? $" #{Id}" : string.Empty);

		public bool Equals(RequestMessage other) =>
			other != null &&
			Equals(Version, other.Version) &&
			Equals(Name, other.Name) &&
			Equals(Parameters, other.Parameters) &&
			Equals(Id, other.Id);

		public override bool Equals(object obj) => Equals(obj as RequestMessage);

		public override int GetHashCode() =>
			(Name ?? string.Empty).GetHashCode() ^ (Id ?? string.Empty).GetHashCode();
	}
}
