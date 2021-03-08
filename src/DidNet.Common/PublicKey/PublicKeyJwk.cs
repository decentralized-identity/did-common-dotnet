using System.Diagnostics;
using System.Runtime.Serialization;

namespace DidNet.Common.PublicKey
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [DebuggerDisplay("PublicKeyJwk(Crv = {Crv}, Kid = {Kid}, Kty = {Kty}, X = {X}, Y = {Y})")]
    [DataContract]
    public class PublicKeyJwk : KeyFormat, IPublicKeyJwk
    {
        [DataMember(Name ="crv")]
        public virtual string? Crv { get; set; }

        [DataMember(Name ="kid")]
        public virtual string? Kid { get; set; }

        [DataMember(Name ="kty")]
        public virtual string? Kty { get; set; }

        [DataMember(Name ="x")]
        public virtual string? X { get; set; }

        [DataMember(Name ="y")]
        public virtual string? Y { get; set; }
    }
}

