using System.Diagnostics;

namespace DidNet.Common.Verification
{
    /// <summary>
    /// https://www.w3.org/TR/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("CapabilityDelegationMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class CapabilityDelegationMethod : VerificationRelationship, ICapabilityDelegationMethod
    {
        public CapabilityDelegationMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public CapabilityDelegationMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }
}

