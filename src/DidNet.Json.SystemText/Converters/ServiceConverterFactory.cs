using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DidNet.Common;

namespace DidNet.Json.SystemText
{
    /// <summary>
    /// Abc.
    /// TODO: Customize this if more extensive type mapping and converter selection is needed.
    /// </summary>
    public class ServiceConverterFactory: JsonConverterFactory
    {
        /// <summary>
        /// TODO: What are the standardized services in https://www.w3.org/TR/did-spec-registries/ and could be here? The rest ought to be moved out.
        /// When refactoring TODO, retain this observation: Service needs to be always handled unless the library user explicitly removes it.
        /// </summary>
        public static ImmutableDictionary<string, Type> DefaultTypeMap => new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) { { nameof(Service), typeof(Service) } }.ToImmutableDictionary();

        private ImmutableDictionary<string, Type> TypeMap { get; }

        public ServiceConverterFactory(): this(DefaultTypeMap) { }

        public ServiceConverterFactory(ImmutableDictionary<string, Type> typeMap)
        {
            TypeMap = typeMap;
        }


        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            //TODO: This requires there is Service derived on the TypeMap! So this may not be needed then? Either add here or have a fallback as a service
            //with AdditonalData always if TypeMap fails and no Service is otherwise found?
            return TypeMap.Values.Any(mappedServiceType => mappedServiceType.IsAssignableFrom(typeToConvert));
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
}