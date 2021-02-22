using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DidNet.Common
{
    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#context
    /// </summary>
    [DataContract]
    public class Context
    {
        [DataMember(Name="@context")]
        public ICollection<string>? Contexes { get; set; }


        public IDictionary<string, object>? AdditionalData { get; set; }
    }
}

