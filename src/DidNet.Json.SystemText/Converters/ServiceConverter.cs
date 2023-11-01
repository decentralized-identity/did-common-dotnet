using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using DidNet.Common;
using DidNet.Json.SystemText.Converters;
using DidNet.Json.SystemText.ModelExt;

namespace DidNet.Json.SystemText
{
    /// <summary>
    /// Converts a <see cref="DidNet.Common.Service"/> derived object to and from JSON.
    /// </summary>
    /// <typeparam name="T">A service type to convert.</typeparam>
    public class ServiceConverter<T> : JsonConverter<T> where T : IService
    {
        /// <summary>
        /// A runtime map of <see cref="DidNet.Common.Service"/> and sub-types.
        /// </summary>
        private ImmutableDictionary<string, Type> TypeMap { get; }


        /// <summary>
        /// A default constructor for <see cref="DidNet.Common.Service"/> and sub-type conversions.
        /// </summary>
        /// <param name="typeMap">A runtime map of of <see cref="DidNet.Common.Service"/> and sub-types.</param>
        public ServiceConverter(ImmutableDictionary<string, Type> typeMap)
        {
            TypeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
        }


        /// <inheritdoc/>
        [return: NotNull]
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                ThrowHelper.ThrowJsonException();
            }

            //Parsing the document forwards moves the index. The element start position
            //is stored to a temporary variable here so it can be given directly to JsonSerializer.
            var elementStartPosition = reader;
            using (var jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                //TODO: This discriminator can be lifted and this converter generalized further.
                //While the Factory can instantiate the generalized version with a specific
                //discriminator and type map.
                const string ServiceTypeDiscriminator = "type";
                var serviceElement = jsonDocument.RootElement;
                var serviceType = serviceElement.GetProperty(ServiceTypeDiscriminator).GetString();

                var namePolicyOptions = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new TypeMappingConverter<IService, ServiceExt>(),
                        new ServiceEndpointDataConverter(),
                    },
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                if (!string.IsNullOrEmpty(serviceType))
                {
                    if (TypeMap.TryGetValue(serviceType, out var targetType))
                    {
                        return (T)JsonSerializer.Deserialize(ref elementStartPosition, targetType, namePolicyOptions)!;
                    }

                    var service = JsonSerializer.Deserialize<IService>(ref elementStartPosition, namePolicyOptions)!;

                    return (T)service;
                }

                throw new JsonException($"No handler for service \"{serviceType}\" found.");
            }
        }


        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var namePolicyOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new TypeMappingConverter<IService, ServiceExt>(),
                    new ServiceEndpointDataConverter(),
                },

                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            JsonSerializer.Serialize(writer, value, value.GetType(), namePolicyOptions);
        }
    }
}