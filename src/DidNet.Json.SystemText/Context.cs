using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace DidNet.Common
{

    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#context
    /// </summary>
    [DataContract]
    public class Context : IContext
    {
        [JsonPropertyName("@context")]
        public ICollection<string>? Contexes { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object>? AdditionalData { get; set; }
    }
}

