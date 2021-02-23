using System.Diagnostics;
using DidNet.Common.Verification;

namespace DidNet.Json.Newtonsoft.Verification
{
    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("KeyAgreementMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class KeyAgreementMethod : VerificationRelationship, IKeyAgreementMethod
    {
        public KeyAgreementMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public KeyAgreementMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }
}

