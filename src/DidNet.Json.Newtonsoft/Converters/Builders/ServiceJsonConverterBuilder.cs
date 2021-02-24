using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DidNet.Common;
using DidNet.Json.Newtonsoft.ModelExt;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft.Converters.Builders
{
    public class ServiceJsonConverterBuilder : IJsonConverterBuilder
    {
        public static ImmutableDictionary<string, Type> DefaultTypeMap => new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase) { { nameof(IService), typeof(ServiceExt) } }.ToImmutableDictionary();
        private readonly Dictionary<string, Type>? typeMaps = new();

        public ServiceJsonConverterBuilder AddOrUpdateMapping(string typeName, Type type)
        {
            if (typeMaps!.ContainsKey(typeName))
            {
                typeMaps[typeName] = type;
            }
            else
            {
                typeMaps.Add(typeName, type);
            }

            return this;
        }

        public ServiceJsonConverterBuilder AddOrUpdateMappings(Dictionary<string, Type> mappings)
        {
            foreach (var map in mappings)
            {
                AddOrUpdateMapping(map.Key, map.Value);
            }

            return this;
        }

        public ServiceJsonConverterBuilder AddOrUpdateMapping<T>(string typeName)
        {
            AddOrUpdateMapping(typeName, typeof(T));
            return this;
        }

        public IEnumerable<JsonConverter> GetConverters()
        {
            return new JsonConverterCollection()
            {
                new ServiceConverter<ServiceExt>(typeMaps == null || typeMaps.Count == 0 ? DefaultTypeMap: typeMaps.ToImmutableDictionary()),
            };
        }
    }
}