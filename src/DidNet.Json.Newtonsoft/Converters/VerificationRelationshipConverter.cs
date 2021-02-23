using DidNet.Common.Verification;
using System;
using System.Collections.Generic;
using DidNet.Json.Newtonsoft.Verification;
using Newtonsoft.Json;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).ullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

namespace DidNet.Json.Newtonsoft.Converters
{
    public class VerificationRelationshipConverter<TVerificationRelationship>: JsonConverter<TVerificationRelationship> where TVerificationRelationship : IVerificationRelationship
    {

        private readonly Dictionary<Type, Type> implementationMapping = new();

        public VerificationRelationshipConverter()
        {
            //TODO something better
            implementationMapping.Add(typeof(IAssertionMethod), typeof(AssertionMethod));
            implementationMapping.Add(typeof(IAuthenticationMethod), typeof(AuthenticationMethod));
            implementationMapping.Add(typeof(ICapabilityDelegationMethod), typeof(CapabilityDelegationMethod));
            implementationMapping.Add(typeof(ICapabilityInvocationMethod), typeof(CapabilityInvocationMethod));
            implementationMapping.Add(typeof(IKeyAgreementMethod), typeof(KeyAgreementMethod));
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

            var implementation = implementationMapping[typeof(TVerificationRelationship)];


            return (TVerificationRelationship) Activator.CreateInstance(implementation,
                new object[] {constructorParameter!})!;
        }

    }
}
