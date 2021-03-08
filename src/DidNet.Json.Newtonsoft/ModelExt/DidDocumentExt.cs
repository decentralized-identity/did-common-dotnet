using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using DidNet.Common;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft.ModelExt
{

    /// <summary>
    /// https://w3c.github.io/did-core/
    /// </summary>
    [DebuggerDisplay("DidDocument(Id = {Id})")]
    [DataContract]
    public class DidDocumentExt :DidDocument
    {
        [JsonExtensionData] public override IDictionary<string, object>? AdditionalData { get; set; }
    }
}

