using System.Diagnostics;

namespace DidNet.Common.Verification
{
    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("AuthenticationMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class AuthenticationMethod : VerificationRelationship, IAuthenticationMethod
    {
        public AuthenticationMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public AuthenticationMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }
}

