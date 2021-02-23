using System.Diagnostics;
using System.Runtime.Serialization;
using DidNet.Common.PublicKey;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft.PublicKey
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [DebuggerDisplay("PublicKeyJwk(Crv = {Crv}, Kid = {Kid}, Kty = {Kty}, X = {X}, Y = {Y})")]
    [DataContract]
    public class PublicKeyJwk : KeyFormat, IPublicKeyJwk
    {
        [DataMember(Name ="crv")]

        public string Crv { get; set; }

        [DataMember(Name ="kid")]
        [JsonProperty("kid", NullValueHandling = NullValueHandling.Ignore)]
        public string Kid { get; set; }

        [DataMember(Name ="kty")]
        public string Kty { get; set; }

        [DataMember(Name ="x")]
        public string X { get; set; }

        [DataMember(Name ="y")]
        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public string Y { get; set; }
    }
}

