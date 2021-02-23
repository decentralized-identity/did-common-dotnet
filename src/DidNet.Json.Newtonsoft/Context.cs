using System.Collections.Generic;
using System.Runtime.Serialization;
using DidNet.Common;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft
{

    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#context
    /// </summary>
    [DataContract]
    public class Context : IContext
    {
        [DataMember(Name="@context")]
        public ICollection<string>? Contexes { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object>? AdditionalData { get; set; }
    }
}

