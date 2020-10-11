using DotDecentralized.Core.Did;
using System.Text.Json.Serialization;

namespace DotDecentralized.Tests
{
    //TODO: Work in progress.
    //Quickly done some DTOs to test serialization of more specialized services in DidDocumentTests.

    public class OpenIdConnectVersion1: Service
    { }


    public class SpamCost
    {
        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    public class SocialWebInboxService: Service
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("spamCost")]
        public SpamCost? SpamCost { get; set; }
    }


    public class VerifiableCredentialService: Service
    {
    }
}
