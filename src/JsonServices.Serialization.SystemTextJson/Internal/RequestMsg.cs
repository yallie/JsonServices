using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace JsonServices.Serialization.SystemTextJson.Internal
{
	internal class RequestMsg<T> : IRequestMessage
	{
		[JsonPropertyName("params")]
		public T Parameters { get; set; }
		object IRequestMessage.Parameters => Parameters;
	}
}
