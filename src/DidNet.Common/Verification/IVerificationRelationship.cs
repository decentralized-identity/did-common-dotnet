namespace DidNet.Common.Verification
{
    public interface IVerificationRelationship
    {
        string? VerificationReferenceId { get; }
        IVerificationMethod? EmbeddedVerification { get; }
        string? Id { get; }
        bool IsEmbeddedVerification { get; }
    }
}