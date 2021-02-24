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
            if (value?.Contexes?.Count == 1)
            {
                writer.WriteValue(value.Contexes.ElementAt(0));
            }
            else if (value?.Contexes?.Count > 1)
            {
                writer.WriteStartArray();
                for (var i = 0; i < value?.Contexes.Count; ++i)
                {
                    writer.WriteValue(value.Contexes.ElementAt(i));
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
            var context = new TContext();
         
            if (reader.TokenType == JsonToken.String)
            {
                context.Contexes = new List<string>();
                var singleContext = serializer.Deserialize<string>(reader);
                //Should check and validate the context name to be "https://www.w3.org/ns/did/v1"
                if (singleContext != null)
                {
                    context.Contexes.Add(singleContext);
                }

                return context;
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                var multipleContexts = serializer.Deserialize<string[]>(reader);
                //Should check and validate the context name to be "https://www.w3.org/ns/did/v1"
                if (multipleContexts != null)
                {
                    context.Contexes = new List<string>(multipleContexts);
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
    }
}
