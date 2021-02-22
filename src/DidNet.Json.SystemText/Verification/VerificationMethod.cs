using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using DidNet.Common.PublicKey;

namespace DidNet.Common.Verification
{
    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("VerificationMethod(Id = {Id})")]
    public class VerificationMethod: IVerificationMethod
    {
        //TODO: Could be FractionOrUri: Uri, or C# 10/F# discriminated union (like VerificationRelationship would be).
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("controller")]
        public string? Controller { get; set; }

        public IKeyFormat? KeyFormat { get; set; }
    }
}

