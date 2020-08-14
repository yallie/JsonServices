using System.Text.Json.Serialization;
using JsonServices.Messages;

namespace JsonServices.Serialization.SystemTextJson.Internal
{
	internal class GenericMessage
	{
		[JsonPropertyName("jsonrpc")]
		public string Version { get; set; }

		[JsonPropertyName("method")]
		public string Name { get; set; }

		[JsonPropertyName("error")]
		public Error Error { get; set; }

		[JsonPropertyName("id")]
		public string Id { get; set; }

		public bool IsValid => Version == "2.0" &&
			(!string.IsNullOrWhiteSpace(Name) || !string.IsNullOrWhiteSpace(Id));
	}
}
