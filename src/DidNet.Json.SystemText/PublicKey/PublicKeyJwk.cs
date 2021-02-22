using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DidNet.Common.PublicKey
{
    [DebuggerDisplay("PublicKeyJwk(Crv = {Crv}, Kid = {Kid}, Kty = {Kty}, X = {X}, Y = {Y})")]
    public class PublicKeyJwk : KeyFormat, IPublicKeyJwk
    {
        [JsonPropertyName("crv")]
        public string? Crv { get; set; }

        [JsonPropertyName("kid")]
        public string? Kid { get; set; }

        [JsonPropertyName("kty")]
        public string? Kty { get; set; }

        [JsonPropertyName("x")]
        public string? X { get; set; }

        [JsonPropertyName("y")]
        public string? Y { get; set; }
    }
}

