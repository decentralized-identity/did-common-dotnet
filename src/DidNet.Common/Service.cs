using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DidNet.Common
{

    /// <summary>
    /// https://www.w3.org/TR/did-core/#service-endpoints
    /// </summary>
    [DebuggerDisplay("Service(Id = {Id})")]
    [DataContract]
    public class Service : IService
    {
        [DataMember(Name ="id")]
        public virtual Uri? Id { get; set; }

        [DataMember(Name ="type")]
        public virtual string? Type { get; set; }

        [DataMember(Name ="serviceEndpoint")]
        public virtual ServiceEndpointData? ServiceEndpoint { get; set; }

        public virtual IDictionary<string, object>? AdditionalData { get; set; }
    }
}

