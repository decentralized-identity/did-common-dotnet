using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DidNet.Common.Tests.DidJsonParsing.SystemText
{
    //TODO: Work in progress.
    //Quickly done some DTOs to test serialization of more specialized services in DidDocumentTests.

    [DebuggerDisplay("OpenIdConnectVersion1(Id = {Id})")]
    public class OpenIdConnectVersion1: Service
    { }


    [DebuggerDisplay("SpamCost(Amount = {Amount}, Currency = {Currency})")]
    public class SpamCost
    {
        [DataMember(Name="amount")]
        public string? Amount { get; set; }

        [DataMember(Name = "currency")]
        public string? Currency { get; set; }
    }

    [DebuggerDisplay("SocialWebInboxService(Id = {Id})")]
    public class SocialWebInboxService: Service
    {
        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "spamCost")]
        public SpamCost? SpamCost { get; set; }
    }


    [DebuggerDisplay("VerifiableCredentialService(Id = {Id})")]
    public class VerifiableCredentialService: Service
    {
    }
}
