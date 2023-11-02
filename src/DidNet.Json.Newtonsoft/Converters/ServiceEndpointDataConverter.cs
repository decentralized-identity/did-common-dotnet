using DidNet.Common;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace DidNet.Json.Newtonsoft.Converters
{
    public class ServiceEndpointDataConverter : JsonConverter<ServiceEndpointData>
    {
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        public override ServiceEndpointData? ReadJson(JsonReader reader, Type objectType, [AllowNull] ServiceEndpointData existingValue, bool hasExistingValue, JsonSerializer serializer)
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        {
            ServiceEndpointData? serviceEndpointObj = null;
            var tokenType = reader.TokenType;
            if (reader.TokenType == JsonToken.String)
            {
                var serviceEndpoint = serializer.Deserialize<string>(reader);
                
                if (serviceEndpoint != null)
                {
                    serviceEndpointObj = new ServiceEndpointData(serviceEndpoint);
                }

                return serviceEndpointObj;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var serviceEndpoint = serializer.Deserialize<Dictionary<string, List<string>>>(reader);
                if (serviceEndpoint != null)
                {
                    serviceEndpointObj = new ServiceEndpointData(serviceEndpoint);
                }

                return serviceEndpointObj;
            }
            return serviceEndpointObj;

        }

        public override void WriteJson(JsonWriter writer, [AllowNull] ServiceEndpointData value, JsonSerializer serializer)
        {
            if(value != null)
            {
                if (value.IsEmbeddedEndpoint && value.EmbeddedEndpoint != null)
                {
                    serializer.Serialize(writer, value.EmbeddedEndpoint);
                }
                else if (!value.IsEmbeddedEndpoint)
                {
                    writer.WriteValue(value.Endpoint);
                }
            }
        }
    }
}