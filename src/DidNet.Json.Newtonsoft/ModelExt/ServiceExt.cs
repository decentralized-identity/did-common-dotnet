using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using DidNet.Common;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft.ModelExt
{

    /// <summary>
    /// https://www.w3.org/TR/did-core/#service-endpoints
    /// </summary>
    [DebuggerDisplay("Service(Id = {Id})")]
    [DataContract]
    public class ServiceExt : Service
    {
        [JsonExtensionData]
        public override IDictionary<string, object>? AdditionalData { get; set; }
    }
}

