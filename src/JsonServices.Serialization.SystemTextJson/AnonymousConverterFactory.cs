using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonServices.Serialization.SystemTextJson
{
	/// <summary>
	/// Converter factory for the anonymous types.
	/// </summary>
	internal class AnonymousConverterFactory : JsonConverterFactory
	{
		public override bool CanConvert(Type t)
		{
			// anonymous types are always generic and [CompilerGenerated]
			if (!t.IsGenericType || !Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute), false))
			{
				return false;
			}

			// Microsoft C# anonymous type
			if (t.Name.StartsWith("<>f__AnonymousType"))
			{
				return true;
			}

			// Microsoft Visual Basic anonymous type
			if (t.Name.StartsWith("VB$AnonymousType"))
			{
				return true;
			}

			// Mono C# anonymous type
			if (t.Name.StartsWith("<>__AnonType"))
			{
				return true;
			}

			return false;
		}

		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
			var jsonConverterType = typeof(AnonConverter<>).MakeGenericType(typeToConvert);
			return (JsonConverter)Activator.CreateInstance(jsonConverterType);
		}

		private class AnonConverter<T> : JsonConverter<T>
		{
			private static PropertyInfo[] Properties { get; } = typeof(T).GetProperties();

			public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var args = new List<object>();
				if (!reader.Read())
				{
					throw new JsonException();
				}

				while (reader.TokenType != JsonTokenType.EndObject)
				{
					foreach (var prop in Properties)
					{
						if (reader.ValueTextEquals(prop.Name) && reader.Read())
						{
							var arg = JsonSerializer.Deserialize(ref reader, prop.PropertyType, options);
							args.Add(arg);
						}
					}

					reader.Read();
				}

				return (T)Activator.CreateInstance(typeof(T), args.ToArray());
			}

			public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
			{
				writer.WriteStartObject();

				foreach (var prop in Properties)
				{
					writer.WritePropertyName(prop.Name);
					JsonSerializer.Serialize(writer, prop.GetValue(value), options);
				}

				writer.WriteEndObject();
			}
		}
	}
}
