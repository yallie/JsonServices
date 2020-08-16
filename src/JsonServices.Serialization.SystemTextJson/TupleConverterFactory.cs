using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonServices.Serialization.SystemTextJson
{
	/// <summary>
	/// Helps deserializing tuples.
	/// Inspiration: https://github.com/dotnet/runtime/issues/1519#issuecomment-572751931
	/// </summary>
	internal class TupleConverterFactory : JsonConverterFactory
	{
		private Type[] TupleTypes { get; } = new[]
		{
			typeof(Tuple<>),
			typeof(Tuple<,>), typeof(Tuple<,,>),
			typeof(Tuple<,,,>), typeof(Tuple<,,,,>),
			typeof(Tuple<,,,,,>), typeof(Tuple<,,,,,,>),
			typeof(ValueTuple<>),
			typeof(ValueTuple<,>), typeof(ValueTuple<,,>),
			typeof(ValueTuple<,,,>), typeof(ValueTuple<,,,,>),
			typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>),
		};

		public override bool CanConvert(Type type) =>
			type.IsGenericType && TupleTypes.Contains(type.GetGenericTypeDefinition());

		private Type GetConverterType(Type type)
		{
			var args = type.GetGenericArguments();
			if (type.Name.StartsWith(nameof(ValueTuple)))
			{
				switch (args.Length)
				{
					case 1:
						return typeof(ValueTupleConverter<>).MakeGenericType(args);

					case 2:
						return typeof(ValueTupleConverter<,>).MakeGenericType(args);

					case 3:
						return typeof(ValueTupleConverter<,,>).MakeGenericType(args);

					case 4:
						return typeof(ValueTupleConverter<,,,>).MakeGenericType(args);

					case 5:
						return typeof(ValueTupleConverter<,,,,>).MakeGenericType(args);

					case 6:
						return typeof(ValueTupleConverter<,,,,,>).MakeGenericType(args);

					case 7:
						return typeof(ValueTupleConverter<,,,,,,>).MakeGenericType(args);

					default:
						throw new NotSupportedException();
				}
			}
			else
			{
				switch (args.Length)
				{
					case 1:
						return typeof(TupleConverter<>).MakeGenericType(args);

					case 2:
						return typeof(TupleConverter<,>).MakeGenericType(args);

					case 3:
						return typeof(TupleConverter<,,>).MakeGenericType(args);

					case 4:
						return typeof(TupleConverter<,,,>).MakeGenericType(args);

					case 5:
						return typeof(TupleConverter<,,,,>).MakeGenericType(args);

					case 6:
						return typeof(TupleConverter<,,,,,>).MakeGenericType(args);

					case 7:
						return typeof(TupleConverter<,,,,,,>).MakeGenericType(args);

					default:
						throw new NotSupportedException();
				}
			}
		}

		public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options) =>
			(JsonConverter)Activator.CreateInstance(GetConverterType(type));

		private static ValueTuple<T1, T2, T3, T4, T5, T6, T7> ReadTuple<T1, T2, T3, T4, T5, T6, T7>(ref Utf8JsonReader reader, JsonSerializerOptions options)
		{
			(T1, T2, T3, T4, T5, T6, T7) result = default;
			if (!reader.Read())
			{
				throw new JsonException();
			}

			while (reader.TokenType != JsonTokenType.EndObject)
			{
				if (reader.ValueTextEquals(nameof(result.Item1)) && reader.Read())
				{
					result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
				}
				else if (reader.ValueTextEquals(nameof(result.Item2)) && reader.Read())
				{
					result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
				}
				else if (reader.ValueTextEquals(nameof(result.Item3)) && reader.Read())
				{
					result.Item3 = JsonSerializer.Deserialize<T3>(ref reader, options);
				}
				else if (reader.ValueTextEquals(nameof(result.Item4)) && reader.Read())
				{
					result.Item4 = JsonSerializer.Deserialize<T4>(ref reader, options);
				}
				else if (reader.ValueTextEquals(nameof(result.Item5)) && reader.Read())
				{
					result.Item5 = JsonSerializer.Deserialize<T5>(ref reader, options);
				}
				else if (reader.ValueTextEquals(nameof(result.Item6)) && reader.Read())
				{
					result.Item6 = JsonSerializer.Deserialize<T6>(ref reader, options);
				}
				else if (reader.ValueTextEquals(nameof(result.Item7)) && reader.Read())
				{
					result.Item7 = JsonSerializer.Deserialize<T7>(ref reader, options);
				}
				else
				{
					throw new JsonException();
				}

				reader.Read();
			}

			return result;
		}

		private static void WriteTuple<T1, T2, T3, T4, T5, T6, T7>(int count, Utf8JsonWriter writer, (T1, T2, T3, T4, T5, T6, T7) tuple, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(tuple.Item1));
			JsonSerializer.Serialize(writer, tuple.Item1, options);
			if (count > 1)
			{
				writer.WritePropertyName(nameof(tuple.Item2));
				JsonSerializer.Serialize(writer, tuple.Item2, options);
				if (count > 2)
				{
					writer.WritePropertyName(nameof(tuple.Item3));
					JsonSerializer.Serialize(writer, tuple.Item3, options);
					if (count > 3)
					{
						writer.WritePropertyName(nameof(tuple.Item4));
						JsonSerializer.Serialize(writer, tuple.Item4, options);
						if (count > 4)
						{
							writer.WritePropertyName(nameof(tuple.Item5));
							JsonSerializer.Serialize(writer, tuple.Item5, options);
							if (count > 5)
							{
								writer.WritePropertyName(nameof(tuple.Item6));
								JsonSerializer.Serialize(writer, tuple.Item6, options);
								if (count > 6)
								{
									writer.WritePropertyName(nameof(tuple.Item7));
									JsonSerializer.Serialize(writer, tuple.Item7, options);
								}
							}
						}
					}
				}
			}

			writer.WriteEndObject();
		}

		private class TupleConverter<T1> : JsonConverter<Tuple<T1>>
		{
			public override Tuple<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, object, object, object, object, object, object>(ref reader, options);
				return Tuple.Create(result.Item1);
			}

			public override void Write(Utf8JsonWriter writer, Tuple<T1> value, JsonSerializerOptions options) =>
				WriteTuple(1, writer, (value.Item1, 0, 0, 0, 0, 0, 0), options);
		}

		private class TupleConverter<T1, T2> : JsonConverter<Tuple<T1, T2>>
		{
			public override Tuple<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, object, object, object, object, object>(ref reader, options);
				return Tuple.Create(result.Item1, result.Item2);
			}

			public override void Write(Utf8JsonWriter writer, Tuple<T1, T2> value, JsonSerializerOptions options) =>
				WriteTuple(2, writer, (value.Item1, value.Item2, 0, 0, 0, 0, 0), options);
		}

		private class TupleConverter<T1, T2, T3> : JsonConverter<Tuple<T1, T2, T3>>
		{
			public override Tuple<T1, T2, T3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, object, object, object, object>(ref reader, options);
				return Tuple.Create(result.Item1, result.Item2, result.Item3);
			}

			public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3> value, JsonSerializerOptions options) =>
				WriteTuple(3, writer, (value.Item1, value.Item2, value.Item3, 0, 0, 0, 0), options);
		}

		private class TupleConverter<T1, T2, T3, T4> : JsonConverter<Tuple<T1, T2, T3, T4>>
		{
			public override Tuple<T1, T2, T3, T4> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, object, object, object>(ref reader, options);
				return Tuple.Create(result.Item1, result.Item2, result.Item3, result.Item4);
			}

			public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4> value, JsonSerializerOptions options) =>
				WriteTuple(4, writer, (value.Item1, value.Item2, value.Item3, value.Item4, 0, 0, 0), options);
		}

		private class TupleConverter<T1, T2, T3, T4, T5> : JsonConverter<Tuple<T1, T2, T3, T4, T5>>
		{
			public override Tuple<T1, T2, T3, T4, T5> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, T5, object, object>(ref reader, options);
				return Tuple.Create(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5);
			}

			public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5> value, JsonSerializerOptions options) =>
				WriteTuple(5, writer, (value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, 0, 0), options);
		}

		private class TupleConverter<T1, T2, T3, T4, T5, T6> : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6>>
		{
			public override Tuple<T1, T2, T3, T4, T5, T6> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, T5, T6, object>(ref reader, options);
				return Tuple.Create(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6);
			}

			public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6> value, JsonSerializerOptions options) =>
				WriteTuple(6, writer, (value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, 0), options);
		}

		private class TupleConverter<T1, T2, T3, T4, T5, T6, T7> : JsonConverter<Tuple<T1, T2, T3, T4, T5, T6, T7>>
		{
			public override Tuple<T1, T2, T3, T4, T5, T6, T7> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, T5, T6, T7>(ref reader, options);
				return Tuple.Create(result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7);
			}

			public override void Write(Utf8JsonWriter writer, Tuple<T1, T2, T3, T4, T5, T6, T7> value, JsonSerializerOptions options) =>
				WriteTuple(7, writer, (value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7), options);
		}

		private class ValueTupleConverter<T1> : JsonConverter<ValueTuple<T1>>
		{
			public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, object, object, object, object, object, object>(ref reader, options);
				return ValueTuple.Create(result.Item1);
			}

			public override void Write(Utf8JsonWriter writer, ValueTuple<T1> value, JsonSerializerOptions options) =>
				WriteTuple(1, writer, (value.Item1, 0, 0, 0, 0, 0, 0), options);
		}

		private class ValueTupleConverter<T1, T2> : JsonConverter<ValueTuple<T1, T2>>
		{
			public override ValueTuple<T1, T2> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, object, object, object, object, object>(ref reader, options);
				return (result.Item1, result.Item2);
			}

			public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2> value, JsonSerializerOptions options) =>
				WriteTuple(2, writer, (value.Item1, value.Item2, 0, 0, 0, 0, 0), options);
		}

		private class ValueTupleConverter<T1, T2, T3> : JsonConverter<ValueTuple<T1, T2, T3>>
		{
			public override ValueTuple<T1, T2, T3> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, object, object, object, object>(ref reader, options);
				return (result.Item1, result.Item2, result.Item3);
			}

			public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3> value, JsonSerializerOptions options) =>
				WriteTuple(3, writer, (value.Item1, value.Item2, value.Item3, 0, 0, 0, 0), options);
		}

		private class ValueTupleConverter<T1, T2, T3, T4> : JsonConverter<ValueTuple<T1, T2, T3, T4>>
		{
			public override ValueTuple<T1, T2, T3, T4> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, object, object, object>(ref reader, options);
				return (result.Item1, result.Item2, result.Item3, result.Item4);
			}

			public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4> value, JsonSerializerOptions options) =>
				WriteTuple(4, writer, (value.Item1, value.Item2, value.Item3, value.Item4, 0, 0, 0), options);
		}

		private class ValueTupleConverter<T1, T2, T3, T4, T5> : JsonConverter<ValueTuple<T1, T2, T3, T4, T5>>
		{
			public override ValueTuple<T1, T2, T3, T4, T5> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, T5, object, object>(ref reader, options);
				return (result.Item1, result.Item2, result.Item3, result.Item4, result.Item5);
			}

			public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5> value, JsonSerializerOptions options) =>
				WriteTuple(5, writer, (value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, 0, 0), options);
		}

		private class ValueTupleConverter<T1, T2, T3, T4, T5, T6> : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6>>
		{
			public override ValueTuple<T1, T2, T3, T4, T5, T6> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, T5, T6, object>(ref reader, options);
				return (result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6);
			}

			public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6> value, JsonSerializerOptions options) =>
				WriteTuple(6, writer, (value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, 0), options);
		}

		private class ValueTupleConverter<T1, T2, T3, T4, T5, T6, T7> : JsonConverter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
		{
			public override ValueTuple<T1, T2, T3, T4, T5, T6, T7> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var result = ReadTuple<T1, T2, T3, T4, T5, T6, T7>(ref reader, options);
				return (result.Item1, result.Item2, result.Item3, result.Item4, result.Item5, result.Item6, result.Item7);
			}

			public override void Write(Utf8JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, JsonSerializerOptions options) =>
				WriteTuple(7, writer, (value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7), options);
		}
	}
}
