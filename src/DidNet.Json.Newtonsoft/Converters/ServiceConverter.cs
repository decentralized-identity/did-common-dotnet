using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DidNet.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).ullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

namespace DidNet.Json.Newtonsoft.Converters
{
    /// <summary>
    /// Converts a <see cref="Service"/> derived object to and from JSON.
    /// </summary>
    /// <typeparam name="TService">A service type to convert.</typeparam>
    public class ServiceConverter: JsonConverter<IService>
    {
        public static ImmutableDictionary<string, Type> DefaultTypeMap => new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) { { nameof(IService), typeof(Service) } }.ToImmutableDictionary();
        /// <summary>
        /// A runtime map of <see cref="Service"/> and sub-types.
        /// </summary>
        private ImmutableDictionary<string, Type> TypeMap { get; }

        /// <summary>
        /// A default constructor for <see cref="Service"/> and sub-type conversions.
        /// </summary>
        /// <param name="typeMap">A runtime map of of <see cref="Service"/> and sub-types.</param>
        public ServiceConverter(ImmutableDictionary<string, Type> typeMap)
        {
            TypeMap = typeMap ?? throw new ArgumentNullException(nameof(typeMap));
        }

        public ServiceConverter():this(DefaultTypeMap)
        {
            
        }

        public override bool CanWrite { get; } = false;

        public override void WriteJson(JsonWriter writer, IService value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (Service)value);
        }

        public override IService ReadJson(JsonReader reader, Type objectType, IService existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            if (reader.TokenType != JsonToken.StartObject)
            {
                //TODO
                throw new JsonReaderException();
            }

            var jObject = JObject.Load(reader);
            
            var serviceType = jObject["type"]?.ToString();


            if (!string.IsNullOrEmpty(serviceType))
            {
                if (TypeMap.TryGetValue(serviceType, out var targetType))
                {
                    return (IService) jObject.ToObject(targetType)!;
                }

                return (IService) jObject.ToObject(typeof(Service))!;
            }
            
            throw new JsonException($"No handler for service \"{serviceType}\" found.");
        }
    }
}
