using System.Diagnostics;

namespace DidNet.Common.Verification
{
    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// The reference Id field is string because it can be a fragment like "#key-1".
    /// </summary>
    [DebuggerDisplay("VerificationRelationship(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public abstract class VerificationRelationship: IVerificationRelationship
    {
        public string? VerificationReferenceId { get; }
        public IVerificationMethod? EmbeddedVerification { get; }

        protected VerificationRelationship(string verificationReferenceId) => VerificationReferenceId = verificationReferenceId;
        protected VerificationRelationship(VerificationMethod embeddedVerification) => EmbeddedVerification = embeddedVerification;

        public string? Id => EmbeddedVerification == null ? VerificationReferenceId : EmbeddedVerification.Id?.ToString();

        public bool IsEmbeddedVerification { get { return EmbeddedVerification != null; } }
    }
}

