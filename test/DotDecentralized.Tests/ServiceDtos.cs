using DotDecentralized.Core.Did;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DotDecentralized.Tests
{
    //TODO: Work in progress.
    //Quickly done some DTOs to test serialization of more specialized services in DidDocumentTests.

    [DebuggerDisplay("OpenIdConnectVersion1(Id = {Id})")]
    public class OpenIdConnectVersion1: Service
    { }


    [DebuggerDisplay("SpamCost(Amount = {Amount}, Currency = {Currency})")]
    public class SpamCost
    {
        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    [DebuggerDisplay("SocialWebInboxService(Id = {Id})")]
    public class SocialWebInboxService: Service
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("spamCost")]
        public SpamCost? SpamCost { get; set; }
    }


    [DebuggerDisplay("VerifiableCredentialService(Id = {Id})")]
    public class VerifiableCredentialService: Service
    {
    }
}
