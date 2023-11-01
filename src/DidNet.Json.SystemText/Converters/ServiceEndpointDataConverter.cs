using DidNet.Common;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DidNet.Json.SystemText.Converters
{
    public class ServiceEndpointDataConverter : JsonConverter<ServiceEndpointData>
    {
        public override ServiceEndpointData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ServiceEndpointData? serviceEndpointObj = null;
            var tokenType = reader.TokenType;
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                if (propertyName == "@context")
                {
                    _ = reader.Read();
                }
            }

            if (tokenType == JsonTokenType.String)
            {
                var serviceEndpoint = reader.GetString();
                if (serviceEndpoint != null)
                {
                    serviceEndpointObj = new ServiceEndpointData(serviceEndpoint);
                }
            }
            else if (tokenType == JsonTokenType.StartObject)
            {
                var serviceEndpoint = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(ref reader);
                if (serviceEndpoint != null)
                {
                    serviceEndpointObj = new ServiceEndpointData(serviceEndpoint);
                }
            }

            return serviceEndpointObj;

        }

        public override void Write(Utf8JsonWriter writer, ServiceEndpointData value, JsonSerializerOptions options)
        {
            var converter = (JsonConverter<IDictionary<string, List<string>>>)options.GetConverter(typeof(IDictionary<string, List<string>>));

            if (value.IsEmbeddedEndpoint && value.EmbeddedEndpoint != null)
            {
                converter.Write(writer, value.EmbeddedEndpoint, options);
            }
            else if (!value.IsEmbeddedEndpoint)
            {
                writer.WriteStringValue(value.Endpoint);
            }
        }
    }
}