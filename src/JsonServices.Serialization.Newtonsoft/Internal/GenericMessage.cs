using System.Runtime.Serialization;
using JsonServices.Messages;

namespace JsonServices.Serialization.Newtonsoft.Internal
{
	[DataContract]
	internal class GenericMessage
	{
		[DataMember(Name = "jsonrpc")]
		public string Version { get; set; }

		[DataMember(Name = "method")]
		public string Name { get; set; }

		[DataMember(Name = "error")]
		public Error Error { get; set; }

		[DataMember(Name = "id")]
		public string Id { get; set; }

		public bool IsValid => Version == "2.0" &&
			(!string.IsNullOrWhiteSpace(Name) || !string.IsNullOrWhiteSpace(Id));

		public bool IsRequest => Name != null;
	}
}
