using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonServices.Serialization.SystemTextJson
{
	public class ObjectConverter : JsonConverter<object>
	{
		public override object Read(ref Utf8JsonReader reader,
			Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.TokenType)
			{
				case JsonTokenType.True:
					return true;

				case JsonTokenType.False:
					return false;

				case JsonTokenType.Number:
					if (reader.TryGetInt32(out var i))
					{
						return i;
					}

					if (reader.TryGetInt64(out var l))
					{
						return l;
					}

					return reader.GetDouble();

				case JsonTokenType.String:
					if (reader.TryGetDateTime(out var datetime))
					{
						return datetime;
					}

					return reader.GetString();

				default:
					using (var document = JsonDocument.ParseValue(ref reader))
					{
						return document.RootElement.Clone();
					}
			}
		}

		public override void Write(Utf8JsonWriter writer,
			object objectToWrite, JsonSerializerOptions options)
		{
			throw new NotSupportedException("Should not get here.");
		}
	}
}
