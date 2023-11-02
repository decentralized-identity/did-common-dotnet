using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using DidNet.Common;

namespace DidNet.Json.SystemText.ModelExt
{

    /// <summary>
    /// https://www.w3.org/TR/did-core/#service-endpoints
    /// </summary>
    [DebuggerDisplay("Service(Id = {Id})")]
    public class ServiceExt : Service
    {
        [JsonPropertyName("id")]
        public override Uri? Id { get; set; }

        [JsonPropertyName("type")]
        public override string? Type { get; set; }

        [JsonPropertyName("serviceEndpoint")]
        public override ServiceEndpointData? ServiceEndpoint { get; set; }

        [JsonExtensionData]
        public override IDictionary<string, object>? AdditionalData { get; set; }
    }
}

