using DidNet.Common.Verification;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).ullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

namespace DidNet.Json.Newtonsoft.Converters
{
    public class VerificationRelationshipConverter<TVerificationRelationship, TVerificationRelationshipImplementation>: JsonConverter<TVerificationRelationship> where TVerificationRelationship : IVerificationRelationship
    where TVerificationRelationshipImplementation: IVerificationRelationship
    {
        public VerificationRelationshipConverter()
        {
        }

        public override void WriteJson(JsonWriter writer, TVerificationRelationship value, JsonSerializer serializer)
        {
            if (value.IsEmbeddedVerification && value.EmbeddedVerification != null)
            {
                serializer.Serialize(writer, value.EmbeddedVerification);
            }
            else if (!value.IsEmbeddedVerification)
            {
                writer.WriteValue(value.VerificationReferenceId);
            }
        }

        public override TVerificationRelationship ReadJson(JsonReader reader, Type objectType,
            TVerificationRelationship existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.StartObject)
            {
                //TODO
                throw new JsonReaderException();
            }

            object? constructorParameter = null;
            if (reader.TokenType == JsonToken.String)
            {
                constructorParameter = serializer.Deserialize<string>(reader) ?? string.Empty;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                constructorParameter = serializer.Deserialize<IVerificationMethod>(reader);
            }
            else
            {
                //TODO
                throw new JsonReaderException();
            }

            return (TVerificationRelationship) Activator.CreateInstance(typeof(TVerificationRelationshipImplementation),
                new object[] {constructorParameter!})!;
        }

    }
}
