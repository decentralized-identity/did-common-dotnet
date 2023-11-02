using System;
using System.Collections.Generic;
using System.Linq;
using DidNet.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DidNet.Json.Newtonsoft.Converters
{
    /// <summary>
    /// Converts <see cref="Context" to and from JSON.
    /// https://w3c.github.io/did-imp-guide/
    /// </summary>
    #pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).ullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public class JsonLdContextConverter<TContext> : JsonConverter<IContext> where TContext: class, IContext, new()
    {
        public override void WriteJson(JsonWriter writer, IContext value, JsonSerializer serializer)
        {
            if (value?.Contexts?.Count == 1)
            {
                WriteContextData(value.Contexts.ElementAt(0), writer, serializer);
            }
            else if (value?.Contexts?.Count > 1)
            {
                writer.WriteStartArray();
                for (var i = 0; i < value?.Contexts.Count; ++i)
                {
                    WriteContextData(value.Contexts.ElementAt(i), writer, serializer);
                }

                writer.WriteEndArray();
            }

            if (value?.AdditionalData?.Count > 0)
            {
                serializer.Serialize(writer, value.AdditionalData);
            }
        }

        public override IContext ReadJson(JsonReader reader, Type objectType, IContext existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var context = new TContext
            {
                Contexts = new List<ContextData>()
            };

            if (reader.TokenType == JsonToken.String)
            {
                var singleContext = serializer.Deserialize<string>(reader);
                //Should check and validate the context name to be "https://www.w3.org/ns/did/v1"
                if (singleContext != null)
                {
                    context.Contexts.Add(new ContextData(singleContext));
                }

                return context;
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                var multipleContexts = serializer.Deserialize<JToken[]>(reader);
                //Should check and validate the context name to be "https://www.w3.org/ns/did/v1"
                if (multipleContexts != null)
                {
                    foreach(var obj in multipleContexts)
                    {
                        if (obj.Type.Equals(JTokenType.String))
                        {
                            context.Contexts.Add(new ContextData(obj.ToString()));
                        }
                        else if (obj.Type.Equals(JTokenType.Object))
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            context.Contexts.Add(new ContextData(obj.ToObject<IDictionary<string, string>>()));
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                        else
                        {
                            throw new JsonException($"Failed to deserialize context of type {obj.Type}");
                        }
                    }
                }

                return context;
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                //Should check and validate the first context name to be "https://www.w3.org/ns/did/v1"
                var jObject = JObject.Load(reader);
                var allData = jObject.ToObject<IDictionary<string, object>>();
                context.AdditionalData = allData;
            }
            return context;

        }
        private void WriteContextData(ContextData contextData, JsonWriter writer, JsonSerializer serializer)
        {
            if (contextData.IsEmbeddedContext && contextData.EmbeddedContext != null)
            {
                serializer.Serialize(writer, contextData.EmbeddedContext);
            }
            else if (!contextData.IsEmbeddedContext)
            {
                writer.WriteValue(contextData.Context);
            }
        }
    }
}