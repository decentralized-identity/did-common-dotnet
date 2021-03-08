using System.Diagnostics;
using System.Runtime.Serialization;
using DidNet.Common.PublicKey;

namespace DidNet.Common.Verification
{
    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("VerificationMethod(Id = {Id})")]
    [DataContract]
    public class VerificationMethod: IVerificationMethod
    {
        //TODO: Could be FractionOrUri: Uri, or C# 10/F# discriminated union (like VerificationRelationship would be).
        [DataMember(Name ="id")]
        public virtual string? Id { get; set; }

        [DataMember(Name ="type")]
        public virtual string? Type { get; set; }

        [DataMember(Name ="controller")]
        public virtual string? Controller { get; set; }

        public IKeyFormat? KeyFormat { get; set; }
    }
}

