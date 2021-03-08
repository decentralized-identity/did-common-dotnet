using DidNet.Common.PublicKey;

namespace DidNet.Common.Verification
{
    public interface IVerificationMethod
    {
        string? Id { get; set; }
        string? Type { get; set; }
        string? Controller { get; set; }
        IKeyFormat? KeyFormat { get; set; }
    }
}