using System.Collections.Generic;
using System.Linq;
using DidNet.Common;
using DidNet.Json.Newtonsoft.Converters;
using DidNet.Json.Newtonsoft.Converters.Builders;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft
{
    public class DidJsonSerializerSettings
    {
        public DidJsonSerializerSettings()
        {
            ServiceConverter = new ServiceJsonConverterBuilder();
            VerificationRelationshipConverter = new VerificationRelationshipConverterBuilder();
        }

        public ServiceJsonConverterBuilder ServiceConverter { get; }
        public VerificationRelationshipConverterBuilder VerificationRelationshipConverter { get; }

        public JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = GetAllConverters().ToList()
            };
        }

        public IEnumerable<JsonConverter> GetAllConverters()
        {
            return ServiceConverter.GetConverters().Concat(VerificationRelationshipConverter.GetConverters())
                .Concat(GetDefaultConverters());
        }

        protected IEnumerable<JsonConverter> GetDefaultConverters()
        {
            return new JsonConverter[]
            {
                new JsonLdContextConverter<ContextObj>(),
                new VerificationMethodConverter(),
                new ServiceConverter<Service>(),
            };
        }
    }
}
