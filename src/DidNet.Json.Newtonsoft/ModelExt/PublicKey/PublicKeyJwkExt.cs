using System.Diagnostics;
using System.Runtime.Serialization;
using DidNet.Common.PublicKey;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft.ModelExt.PublicKey
{
    [DebuggerDisplay("PublicKeyJwk(Crv = {Crv}, Kid = {Kid}, Kty = {Kty}, X = {X}, Y = {Y})")]
    [DataContract]
    public class PublicKeyJwkExt : PublicKeyJwk
    {
        [DataMember(Name ="kid")]
        [JsonProperty("kid", NullValueHandling = NullValueHandling.Ignore)]
        public override string? Kid { get; set; }

        [DataMember(Name ="y")]
        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public override string? Y { get; set; }
    }
}

