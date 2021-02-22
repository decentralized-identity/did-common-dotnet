using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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
        public Uri? Id { get; set; }

        [DataMember(Name ="type")]
        public string? Type { get; set; }

        [DataMember(Name ="serviceEndpoint")]
        public string? ServiceEndpoint { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object>? AdditionalData { get; set; }
    }
}

