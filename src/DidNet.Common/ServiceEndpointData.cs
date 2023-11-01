using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DidNet.Common
{
    public class ServiceEndpointData
    {
        public string? Endpoint { get; }
        public IDictionary<string, List<string>>? EmbeddedEndpoint { get; }

        [JsonIgnore]
        public bool IsEmbeddedEndpoint { get { return EmbeddedEndpoint != null; } }

        public ServiceEndpointData(string endpoint) => Endpoint = endpoint;

        public ServiceEndpointData(IDictionary<string, List<string>> endpoint) => EmbeddedEndpoint = endpoint;
    }
}
