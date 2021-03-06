using DidNet.Common.Verification;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using DidNet.Common;
using DidNet.Common.PublicKey;
using DidNet.Json.SystemText.Converters;

namespace DidNet.Json.SystemText
{
    /// <summary>
    /// Converts DID verifications methods to and from JSON.
    /// </summary>
    public class VerificationMethodConverter: JsonConverter<IVerificationMethod>
    {
        /// <summary>
        /// Default converters for the verification key types and formats.
        /// This can be used as a basis for an extened type map that is
        /// given as a constructor parameter. For for standard defined
        /// verification types and key formats see at
        /// https://w3c.github.io/did-core/#key-types-and-formats.
        /// </summary>
        public static ImmutableDictionary<string, Func<string, JsonSerializerOptions, IKeyFormat>> DefaultTypeMap =>
            new Dictionary<string, Func<string, JsonSerializerOptions, IKeyFormat>>(StringComparer.OrdinalIgnoreCase)
        {
            { "publicKeyBase58", new Func<string, JsonSerializerOptions, IPublicKeyBase58>((json, _) => new PublicKeyBase58(json)) },
            { "publicKeyPem", new Func<string, JsonSerializerOptions, IPublicKeyPem>((json, _) => new PublicKeyPem(json)) },
            { "publicKeyHex", new Func<string, JsonSerializerOptions, IPublicKeyHex>((json, _) => new PublicKeyHex(json)) },
            { "publicKeyJwk", new Func<string, JsonSerializerOptions, IPublicKeyJwk>((json, options) => JsonSerializer.Deserialize<PublicKeyJwk>(json, options)!) }
        }.ToImmutableDictionary();

        /// <summary>
        /// Xyz.
        /// </summary>
        private ImmutableDictionary<string, Func<string, JsonSerializerOptions, IKeyFormat>> TypeMap { get; }


        /// <summary>
        /// A default constructor that maps <see cref="DefaultTypeMap"/> to be used.
        /// </summary>
        public VerificationMethodConverter(): this(DefaultTypeMap) { }


        /// <summary>
        /// A default constructor for <see cref="VerificationMethod"/> and sub-type conversions.
        /// </summary>
        /// <param name="typeMap">A runtime map of <see cref="DidNet.Common.Service"/> and sub-types.</param>
        public VerificationMethodConverter(ImmutableDictionary<string, Func<string, JsonSerializerOptions, IKeyFormat>> typeMap)
        {
            TypeMap =  typeMap ?? throw new ArgumentNullException(nameof(typeMap));
        }


        /// <inheritdoc/>
        public override IVerificationMethod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartObject)
            {
                ThrowHelper.ThrowJsonException();
            }

          

            //Parsing the document forwards moves the index. The element start position
            //is stored to a temporary variable here so it can be given directly to JsonSerializer.
            var verificationMethod = new VerificationMethod();
            using(var jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                var element = jsonDocument.RootElement;

                //First the values are filled to the object.
                verificationMethod.Id = element.GetProperty("id").GetString()!;
                verificationMethod.Controller = element.GetProperty("controller").GetString();
                verificationMethod.Type = element.GetProperty("type").GetString();

                //Then the known key format tags are tested and its corresponding transformation
                //function is used. This is done like this because JSON can contain any format tags
                //supported by DID Core or registry or extended in custom build. So they need to
                //be tried one-by-one.
                //
                //N.B.! Or find a way to read the next property directly!
                foreach(var serviceTypeDiscriminator in TypeMap.Keys)
                {
                    Func<string, JsonSerializerOptions, IKeyFormat> keyFunc;
                    if(element.TryGetProperty(serviceTypeDiscriminator, out JsonElement serviceTypeElement)
                        && TypeMap.TryGetValue(serviceTypeDiscriminator, out keyFunc!))
                    {
                        verificationMethod.KeyFormat = keyFunc(serviceTypeElement.ToString()!, options);
                        return verificationMethod;
                    }
                }
            }

            throw new JsonException($"{nameof(VerificationMethodConverter.Read)} could not find a converter for \"{verificationMethod.Type}\".");
        }


        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, IVerificationMethod value, JsonSerializerOptions options)
        {
            //TODO: Write use TypeMap as KeyFormat Converter so that these need not to be hardcoded like this.
            writer.WriteStartObject();
            writer.WriteString("id", value?.Id?.ToString());
            writer.WriteString("controller", value?.Controller);
            writer.WriteString("type", value?.Type);

            if(value?.KeyFormat is IPublicKeyHex hex)
            {
                writer.WriteString("publicKeyHex", hex?.Key);
            }

            if(value?.KeyFormat is IPublicKeyBase58 base58)
            {
                writer.WriteString("publicKeyBase58", base58?.Key);
            }

            if(value?.KeyFormat is IPublicKeyJwk jwk)
            {
                writer.WriteStartObject("publicKeyJwk");
                writer.WriteString("crv", jwk.Crv);
                writer.WriteString("x", jwk.X);
                if(!string.IsNullOrWhiteSpace(jwk.Y))
                {
                    writer.WriteString("y", jwk.Y);
                }
                writer.WriteString("kty", jwk.Kty);

                if(!string.IsNullOrWhiteSpace(jwk.Kid))
                {
                    writer.WriteString("kid", jwk.Kid);
                }

                writer.WriteEndObject();
            }

            if(value?.KeyFormat is IPublicKeyPem pem)
            {
                writer.WriteString("publicKeyPem", pem?.Key);
            }

            writer.WriteEndObject();
        }
    }
}
