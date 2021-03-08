using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DidNet.Common;
using Newtonsoft.Json;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).ullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
namespace DidNet.Json.Newtonsoft.Converters
{
    public class TypeMappingConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //assume we can convert to anything for now
            return true;
        }

        [return: MaybeNull]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<T>(reader)!;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
