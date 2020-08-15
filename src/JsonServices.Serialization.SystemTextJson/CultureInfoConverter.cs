using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonServices.Serialization.SystemTextJson
{
	/// <summary>
	/// Serializes a <see cref="CultureInfo"/> as a name, like "ru-RU" or "en-US".
	/// </summary>
	public class CultureInfoConverter : JsonConverter<CultureInfo>
	{
		public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.String)
			{
				throw new JsonException("String literal is expected when reading CultureInfo object.");
			}

			return CultureInfo.GetCultureInfo(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value?.Name);
		}
	}
}
