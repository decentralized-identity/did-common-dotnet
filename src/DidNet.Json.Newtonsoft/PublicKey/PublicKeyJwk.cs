using System.Diagnostics;
using System.Runtime.Serialization;

namespace DidNet.Common.PublicKey
{
    [DebuggerDisplay("PublicKeyJwk(Crv = {Crv}, Kid = {Kid}, Kty = {Kty}, X = {X}, Y = {Y})")]
    public class PublicKeyJwk : KeyFormat, IPublicKeyJwk
    {
        [DataMember(Name ="crv")]
        public string? Crv { get; set; }

        [DataMember(Name ="kid")]
        public string? Kid { get; set; }

        [DataMember(Name ="kty")]
        public string? Kty { get; set; }

        [DataMember(Name ="x")]
        public string? X { get; set; }

        [DataMember(Name ="y")]
        public string? Y { get; set; }
    }
}

