using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DidNet.Common.PublicKey;
using DidNet.Common.Verification;
using DidNet.Json.Newtonsoft.PublicKey;
using DidNet.Json.Newtonsoft.Verification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).ullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

namespace DidNet.Json.Newtonsoft.Converters
{
    /// <summary>
    /// Converts DID verifications methods to and from JSON.
    /// </summary>
    public class VerificationMethodConverter : JsonConverter<IVerificationMethod>
    {
        public static ImmutableDictionary<string, Func<JToken, IKeyFormat>> DefaultTypeMap =>
            new Dictionary<string, Func<JToken, IKeyFormat>>(StringComparer.OrdinalIgnoreCase)
            {
                { "publicKeyBase58", jObject => new PublicKeyBase58(jObject.ToString()) },
                { "publicKeyPem", jObject => new PublicKeyPem(jObject.ToString())},
                { "publicKeyHex", jObject => new PublicKeyHex(jObject.ToString()) },
                { "publicKeyJwk", jObject => jObject.ToObject<PublicKeyJwk>()! }

            }.ToImmutableDictionary();

        private ImmutableDictionary<string, Func<JToken, IKeyFormat>>? TypeMap { get; }

        public VerificationMethodConverter() : this(DefaultTypeMap) { }

        public VerificationMethodConverter(ImmutableDictionary<string, Func<JToken, IKeyFormat>> typeMap)
        {
            TypeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
        }

        public override void WriteJson(JsonWriter writer, IVerificationMethod value, JsonSerializer serializer)
        {
            serializer.NullValueHandling = NullValueHandling.Ignore;
            var jObject = new JObject
            {
                { "id", JValue.CreateString(value?.Id) },
                { "controller", JValue.CreateString(value?.Controller) },
                { "type", JValue.CreateString(value?.Type) }
            };

            switch (value?.KeyFormat)
            {
                case IPublicKeyHex hex:
                    jObject.Add("publicKeyHex", JValue.CreateString(hex?.Key));
                    break;
                case IPublicKeyBase58 base58:
                    jObject.Add("publicKeyBase58", JValue.CreateString(base58?.Key));
                    break;
                case IPublicKeyJwk jwk:
                { 
                    var publicKeyJwkObject = JObject.FromObject(jwk);
                    jObject.Add("publicKeyJwk", publicKeyJwkObject);
                    break;
                }
                case IPublicKeyPem pem:
                    jObject.Add("publicKeyPem", JValue.CreateString(pem?.Key));
                    break;
            }
            
            jObject.WriteTo(writer);
        }

        public override IVerificationMethod ReadJson(JsonReader reader, Type objectType, IVerificationMethod existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var verificationMethod = new VerificationMethod
            {
                Id = jObject["id"]?.Value<string>(),
                Controller = jObject["controller"]?.Value<string>(),
                Type = jObject["type"]?.Value<string>()
            };

            if (TypeMap != null)
            {
                foreach (var keyFormatSupported in TypeMap.Keys)
                {
                    var keyFormat = jObject[keyFormatSupported];
                    if (keyFormat != null)
                    { 
                        verificationMethod.KeyFormat = TypeMap[keyFormatSupported](keyFormat);
                        return verificationMethod;
                    }
                }

            }
            throw new JsonException($"{nameof(ReadJson)} could not find a converter for \"{verificationMethod.Type}\".");

        }
    }
}