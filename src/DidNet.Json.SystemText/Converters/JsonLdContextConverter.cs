using DidNet.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DidNet.Json.SystemText
{
    /// <summary>
    /// Converts <see cref="DidNet.Common.Context" to and from JSON.
    /// Based on DictionaryTKeyEnumTValueConverter
    /// at https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to.
    /// https://w3c.github.io/did-imp-guide/
    /// </summary>
    public class JsonLdContextConverter : JsonConverter<IContext>
    {
        public override IContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //The DID JSON-LD context starts either with a single string, array of strings or is an object that can
            //contain whatever elements.
            var context = new Context { Contexes = new Collection<ContextData>(), AdditionalData = new Dictionary<string, object>() };
            var tokenType = reader.TokenType;
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                if (propertyName == "@context")
                {
                    _ = reader.Read();
                }
            }

            if (tokenType == JsonTokenType.String)
            {
                var ctx = reader.GetString();
                if (ctx != null)
                {
                    context.Contexes.Add(new ContextData(ctx));
                }

                return context;
            }

            if (tokenType == JsonTokenType.StartArray)
            {
                var strList = JsonSerializer.Deserialize<JsonElement[]>(ref reader);
                if (strList != null)
                {
                    for (var i = 0; i < strList.Length; i++)
                    {
                        var s = strList[i];
                        if (s.ValueKind.Equals(JsonValueKind.String))
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            context.Contexes.Add(new ContextData(context: s.GetString()));
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                        else if (s.ValueKind.Equals(JsonValueKind.Object))
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            context.Contexes.Add(new ContextData(JsonSerializer.Deserialize<Dictionary<string, string>>(s.GetString())));
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                        else
                        {
                            throw new JsonException($"Failed to deserialize context of type {s.ValueKind}");
                        }
                    }
                }

                return context;
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return context;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("JsonTokenType was not PropertyName");
                }

                var propertyName = reader.GetString();
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new JsonException("Failed to get property name");
                }

                _ = reader.Read();
                var val = ExtractValue(ref reader, propertyName, options);
                if (val != null)
                {
                    context.AdditionalData.Add(propertyName, val);
                }
            }

            return context;
        }


        public override void Write(Utf8JsonWriter writer, IContext value, JsonSerializerOptions options)
        {
            if (value?.Contexes?.Count == 1)
            {
                WriteContextData(value.Contexes.ElementAt(0), writer, options);
            }
            else if (value?.Contexes?.Count > 1)
            {
                writer.WriteStartArray();
                for (var i = 0; i < value?.Contexes.Count; ++i)
                {
                    WriteContextData(value.Contexes.ElementAt(i), writer, options);
                }

                writer.WriteEndArray();
            }

            if (value?.AdditionalData?.Count > 0)
            {
                JsonSerializer.Serialize(writer, value.AdditionalData);
            }
        }

        private void WriteContextData(ContextData contextData, Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            if (contextData.IsEmbeddedContexe && contextData.EmbeddedContexe != null)
            {
                var converter = (JsonConverter<IDictionary<string, string>>)options.GetConverter(typeof(IDictionary<string, string>));
                converter.Write(writer, contextData.EmbeddedContexe, options);
            }
            else if (!contextData.IsEmbeddedContexe)
            {
                writer.WriteStringValue(contextData.Contexe);
            }
        }

        [return: MaybeNull]
        private static object? ExtractValue(ref Utf8JsonReader reader, string propertyName, JsonSerializerOptions options)
        {
            //https://github.com/dotnet/corefx/blob/master/src/System.Text.Json/src/System/Text/Json/Serialization/Converters/JsonValueConverterKeyValuePair.cs
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out var date))
                    {
                        return date;
                    }
                    return reader.GetString();
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var result))
                    {
                        return result;
                    }
                    return reader.GetDecimal();
                case JsonTokenType.StartObject:
                    return JsonSerializer.Deserialize(ref reader, typeof(object));

                case JsonTokenType.StartArray:
                    return JsonSerializer.Deserialize(ref reader, typeof(object[]));
                default:
                    throw new JsonException($"'{reader.TokenType}' is not supported");
            }
        }
    }
}