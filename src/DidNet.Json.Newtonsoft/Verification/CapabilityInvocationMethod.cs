using System.Diagnostics;
using DidNet.Common.Verification;

namespace DidNet.Json.Newtonsoft.Verification
{
    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("CapabilityInvocationMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class CapabilityInvocationMethod : VerificationRelationship, ICapabilityInvocationMethod
    {
        public CapabilityInvocationMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public CapabilityInvocationMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }
}

