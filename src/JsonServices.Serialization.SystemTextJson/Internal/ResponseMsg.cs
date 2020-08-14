using System.Text.Json.Serialization;

namespace JsonServices.Serialization.SystemTextJson.Internal
{
	internal class ResponseMsg<T> : IResponseMessage
	{
		[JsonPropertyName("result")]
		public T Result { get; set; }
		object IResponseMessage.Result => Result;
	}
}
