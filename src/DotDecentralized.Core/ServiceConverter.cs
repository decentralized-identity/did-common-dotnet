using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotDecentralized.Core.Did
{
    /// <summary>
    /// Abc.
    /// TODO: Customize this if more extensive type mapping and converter selection is needed.
    /// </summary>
    public class ServiceConverterFactory: JsonConverterFactory
    {
        /// <summary>
        /// TODO: What are the standardized services in https://www.w3.org/TR/did-spec-registries/ and could be here? The rest ought to be moved out.
        /// </summary>
        public static ImmutableDictionary<string, Type> DefaultTypeMap => new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase).ToImmutableDictionary();

        private ImmutableDictionary<string, Type> TypeMap { get; }

        public ServiceConverterFactory() : this(DefaultTypeMap) { }

        public ServiceConverterFactory(ImmutableDictionary<string, Type> typeMap)
        {
            TypeMap = typeMap;
        }


        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return TypeMap.Values.Any(s => s.IsAssignableFrom(typeToConvert));
        }



        /// <inheritdoc/>
        [return: NotNull]
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            //This will throw rather than throw if creating instance fails.
            //If type could be Nullable<T>, then .CreateInstance could also return null.
            return (JsonConverter)Activator.CreateInstance(
                typeof(ServiceConverter<>)
                    .MakeGenericType(new Type[] { typeToConvert }),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: new object[] { TypeMap },
                    culture: null)!;
        }
    }


    /// <summary>
    /// Converts a <see cref="Service"/> derived object to and from JSON.
    /// </summary>
    /// <typeparam name="T">A service type to convert.</typeparam>
    public class ServiceConverter<T>: JsonConverter<T> where T: Service
    {
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


        /// <inheritdoc/>
        [return: NotNull]
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartObject)
            {
                ThrowHelper.ThrowJsonException();
            }

            //Parsing the document forwards moves the index. The element start position
            //is stored to a temporary variable here so it can be given directly to JsonSerializer.
            var elementStartPosition = reader;
            using(var jsonDocument = JsonDocument.ParseValue(ref reader))
            {
                //TODO: This discriminator can be lifted and this converter generalized further.
                //While the Factory can instantiate the generalized version with a specific
                //discriminator and type map.
                const string ServiceTypeDiscriminator = "type";
                var serviceElement = jsonDocument.RootElement;
                var serviceType = serviceElement.GetProperty(ServiceTypeDiscriminator).GetString();

                if(!string.IsNullOrEmpty(serviceType) && TypeMap.TryGetValue(serviceType, out var targetType))
                {
                    return (T)JsonSerializer.Deserialize(ref elementStartPosition, targetType)!;
                }

                //TODO: Here put to AdditionalData is ther was not a type on a map? Do it via a default
                //Func<T> or straight away?
            }

            //TODO: Throw a more descriptive exception. Also check that getting here fails the tests!
            throw new JsonException();
        }


        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType());
        }
    }
}
