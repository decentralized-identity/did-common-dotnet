using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using DidNet.Common.Tests.DidJsonParsing.SystemText;
using DidNet.Common.Verification;
using DidNet.Json.Newtonsoft.Converters;
using DidNet.Json.Newtonsoft.ModelExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;
using VerificationMethodConverter = DidNet.Json.Newtonsoft.Converters.VerificationMethodConverter;

namespace DidNet.Common.Tests.DidJsonParsing.Newtonsoft
{
    public static class TestInfrastructureConstants
    {
        public const string RelativeTestPath = @"..\..\..\TestDIDDocuments\";

        /// <summary>
        /// Test material is loaded from external files. This check preconditions assumed on them hold.
        /// </summary>
        /// <param name="didDocumentFilename">The filename under test.</param>
        /// <param name="didDocumentFileContents">The contents of the file being tested.</param>
        public static void ThrowIfPreconditionFails(string didDocumentFilename, string didDocumentFileContents)
        {
            if(didDocumentFilename is null)
            {
                throw new ArgumentNullException(nameof(didDocumentFilename));
            }

            if(string.IsNullOrWhiteSpace(didDocumentFileContents))
            {
                throw new ArgumentException($"The test file {didDocumentFilename} must not be empty or null.", nameof(didDocumentFileContents));
            }
        }
    }

    /// <summary>
    /// General DID tests.
    /// </summary>
    public class DidDocumentTests
    {

        /// <summary>
        /// An example combining https://www.w3.org/TR/did-core/#example-19-various-service-endpoints and other pieces.
        /// </summary>
        private string MultiServiceTestDocument { get; } = @"{
            ""@context"": ""https://www.w3.org/ns/did/v1"",
              ""id"": ""did:example:123456789abcdefghi"",
              ""verificationMethod"": [{
                ""id"": ""did:example:123456789abcdefghi#keys-1"",
                ""type"": ""RsaVerificationKey2018"",
                ""controller"": ""did:example:123456789abcdefghi"",
                ""publicKeyPem"": ""-----BEGIN PUBLIC KEY...END PUBLIC KEY-----\r\n""
              }, {
                ""id"": ""did:example:123456789abcdefghi#keys-3"",
                ""type"": ""RsaVerificationKey2018"",
                ""controller"": ""did:example:123456789abcdefghi"",
                ""publicKeyPem"": ""-----BEGIN PUBLIC KEY...END PUBLIC KEY-----\r\n""
               }, {
                  ""id"": ""did:example:123#_Qq0UL2Fq651Q0Fjd6TvnYE-faHiOpRlPVQcY_-tA4A"",
                  ""type"": ""JwsVerificationKey2020"",
                  ""controller"": ""did:example:123"",
                  ""publicKeyJwk"": {
                  ""crv"": ""Ed25519"",
                  ""x"": ""VCpo2LMLhn6iWku8MKvSLg2ZAoC-nlOyPVQaO3FxVeQ"",
                  ""kty"": ""OKP"",
                  ""kid"": ""_Qq0UL2Fq651Q0Fjd6TvnYE-faHiOpRlPVQcY_-tA4A""
                }
              }],
              ""authentication"": [
                ""did:example:123456789abcdefghi#keys-1"",
                ""did:example:123456789abcdefghi#keys-3"",
                {
                  ""id"": ""did:example:123456789abcdefghi#keys-2"",
                  ""type"": ""Ed25519VerificationKey2018"",
                  ""controller"": ""did:example:123456789abcdefghi"",
                  ""publicKeyBase58"": ""H3C2AVvLMv6gmMNam3uVAjZpfkcJCwDwnZn6z3wXmqPV""
                }
              ],
              ""service"": [{
                ""id"": ""did:example:123456789abcdefghi#openid"",
                ""type"": ""OpenIdConnectVersion1.0Service"",
                ""serviceEndpoint"": ""https://openid.example.com/""
                }, {
                ""id"": ""did:example:123456789abcdefghi#vcr"",
                ""type"": ""CredentialRepositoryService"",
                ""serviceEndpoint"": ""https://repository.example.com/service/8377464""
                }, {
                ""id"": ""did:example:123456789abcdefghi#xdi"",
                ""type"": ""XdiService"",
                ""serviceEndpoint"": ""https://xdi.example.com/8377464""
                }, {
                ""id"": ""did:example:123456789abcdefghi#agent"",
                ""type"": ""AgentService"",
                ""serviceEndpoint"": ""https://agent.example.com/8377464""
                }, {
                ""id"": ""did:example:123456789abcdefghi#messages"",
                ""type"": ""MessagingService"",
                ""serviceEndpoint"": ""https://example.com/messages/8377464""
                }, {
                ""id"": ""did:example:123456789abcdefghi#vcs"",
                ""type"": ""VerifiableCredentialService"",
                ""serviceEndpoint"": ""https://example.com/vc/""
                }, {
                ""id"": ""did:example:123456789abcdefghi#inbox"",
                ""type"": ""SocialWebInboxService"",
                ""serviceEndpoint"": ""https://social.example.com/83hfh37dj"",
                ""description"": ""My public social inbox"",
                ""spamCost"": {
                    ""amount"": ""0.50"",
                    ""currency"": ""USD""
                }}, {
                ""id"": ""did:example:123456789abcdefghi#authpush"",
                ""type"": ""DidAuthPushModeVersion1"",
                ""serviceEndpoint"": ""http://auth.example.com/did:example:123456789abcdefg""
              }]
            }";


        /// <summary>
        /// Getting a hash of an empty document. This should not throw.
        /// </summary>
        [Fact]
        public void EmptyDocumentHash()
        {
            _ = new DidDocument().GetHashCode();
        }


        [Fact]
        public void FullDidDocumentTest()
        {
            var serviceTypeMap = new Dictionary<string, Type>(ServiceConverter.DefaultTypeMap)
            {
                { "OpenIdConnectVersion1.0Service", typeof(OpenIdConnectVersion1) },
                { "CredentialRepositoryService", typeof(ServiceExt) },
                { "XdiService", typeof(ServiceExt) },
                { "AgentService", typeof(ServiceExt) },
                { "IdentityHub", typeof(ServiceExt) },
                { "MessagingService", typeof(ServiceExt) },
                { "SocialWebInboxService", typeof(SocialWebInboxService) },
                { "VerifiableCredentialService", typeof(VerifiableCredentialService) },
                { "DidAuthPushModeVersion1", typeof(ServiceExt) }
            };


            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new VerificationRelationshipConverter<IAssertionMethod>(),
                    new VerificationRelationshipConverter<IAuthenticationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityDelegationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityInvocationMethod>(),
                    new VerificationRelationshipConverter<IKeyAgreementMethod>(),
                    new VerificationMethodConverter(),
                    new ServiceConverter(serviceTypeMap.ToImmutableDictionary()),
                    new JsonLdContextConverter<Context>()
                }
            };

            var deseserializedDidDocument = JsonConvert.DeserializeObject<Json.Newtonsoft.ModelExt.DidDocumentExt>(MultiServiceTestDocument, settings);

            string reserializedDidDocument = JsonConvert.SerializeObject(deseserializedDidDocument, settings);

            var reredeseserializedDidDocument = JsonConvert.DeserializeObject<Json.Newtonsoft.ModelExt.DidDocumentExt>(reserializedDidDocument, settings);

            ////All the DID documents need to have an ID and a context. This one needs to have also a strongly type element.
            ////The strongly typed services should be in document order (e.g. not in type map order).
            Assert.NotNull(deseserializedDidDocument?.Id);
            //Assert.NotNull(deseserializedDidDocument?.Context);
            Assert.NotNull(deseserializedDidDocument?.Service);
            Assert.NotNull(reserializedDidDocument);
            Assert.IsType<OpenIdConnectVersion1>(deseserializedDidDocument!.Service![0]);
            Assert.IsType<VerifiableCredentialService>(deseserializedDidDocument!.Service![5]);
            Assert.IsType<SocialWebInboxService>(deseserializedDidDocument!.Service![6]);
            Assert.IsType<ServiceExt>(deseserializedDidDocument!.Service![7]);

            Assert.True(JToken.DeepEquals(JToken.Parse(MultiServiceTestDocument), JToken.Parse(reserializedDidDocument)));
        }


        /// <summary>
        /// The reader should be able to deserialize all these test files correctly.
        /// </summary>
        /// <param name="didDocumentFilename">The DID document data file under test.</param>
        /// <param name="didDocumentFileContents">The DID document data file contents.</param>
        /// <remarks>Compared to <see cref="CanRoundtripDidDocumentWithoutStronglyTypedService(string, string)"/>
        /// this tests provides strong type to see if <see cref="VerifiableCredentialService"/> in particular is serialized.</remarks>
        [Theory]
        [FilesData(TestInfrastructureConstants.RelativeTestPath + "Generic", "did-verifiablecredentialservice-1.json")]
        public void CanRoundtripDidDocumentWithStronglyTypedService(string didDocumentFilename, string didDocumentFileContents)
        {
            TestInfrastructureConstants.ThrowIfPreconditionFails(didDocumentFilename, didDocumentFileContents);

            var serviceTypeMap = new Dictionary<string, Type>(ServiceConverter.DefaultTypeMap)
            {
                { "VerifiableCredentialService", typeof(VerifiableCredentialService) }
            };

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new VerificationRelationshipConverter<IAssertionMethod>(),
                    new VerificationRelationshipConverter<IAuthenticationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityDelegationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityInvocationMethod>(),
                    new VerificationRelationshipConverter<IKeyAgreementMethod>(),
                    new VerificationMethodConverter(),
                    new ServiceConverter(serviceTypeMap.ToImmutableDictionary()),
                    new JsonLdContextConverter<Context>()
                }
            };

            var deseserializedDidDocument = JsonConvert.DeserializeObject<Json.Newtonsoft.ModelExt.DidDocumentExt>(didDocumentFileContents, settings);

            string reserializedDidDocument = JsonConvert.SerializeObject(deseserializedDidDocument, settings);

            var reredeseserializedDidDocument = JsonConvert.DeserializeObject<Json.Newtonsoft.ModelExt.DidDocumentExt>(reserializedDidDocument, settings);

            //All the DID documents need to have an ID and a context. This one needs to have also a strongly type element.
            Assert.NotNull(deseserializedDidDocument?.Id);
           // Assert.NotNull(deseserializedDidDocument?.Context);
            Assert.NotNull(deseserializedDidDocument?.Service);
            Assert.NotNull(reserializedDidDocument);
            Assert.IsType<VerifiableCredentialService>(deseserializedDidDocument!.Service![0]);

            Assert.True(JToken.DeepEquals(JToken.Parse(didDocumentFileContents), JToken.Parse(reserializedDidDocument)));
        }


        /// <summary>
        /// The reader should be able to deserialize all these test files correctly.
        /// </summary>
        /// <param name="didDocumentFilename">The DID document data file under test.</param>
        /// <param name="didDocumentFileContents">The DID document data file contents.</param>
        /// <remarks>Compared to <see cref="CanRoundtripDidDocumentWithStronglyTypedService(string, string)"/>
        /// this tests without a provided strong type to see if <see cref="Service"/> is serialized.</remarks>
        [Theory]
        [FilesData(TestInfrastructureConstants.RelativeTestPath + "Generic", "did-verifiablecredentialservice-1.json")]
        public void CanRoundtripDidDocumentWithoutStronglyTypedService(string didDocumentFilename, string didDocumentFileContents)
        {
            TestInfrastructureConstants.ThrowIfPreconditionFails(didDocumentFilename, didDocumentFileContents);

            var serviceTypeMap = new Dictionary<string, Type>(ServiceConverter.DefaultTypeMap);
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new VerificationRelationshipConverter<IAssertionMethod>(),
                    new VerificationRelationshipConverter<IAuthenticationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityDelegationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityInvocationMethod>(),
                    new VerificationRelationshipConverter<IKeyAgreementMethod>(),
                    new VerificationMethodConverter(),
                    new ServiceConverter(serviceTypeMap.ToImmutableDictionary()),
                    new JsonLdContextConverter<Context>()
                }
            };

            var deseserializedDidDocument = JsonConvert.DeserializeObject<Json.Newtonsoft.ModelExt.DidDocumentExt>(didDocumentFileContents, settings);

            string reserializedDidDocument = JsonConvert.SerializeObject(deseserializedDidDocument, settings);

            //All the DID documents need to have an ID and a context. This one needs to have also a strongly type element.
            Assert.NotNull(deseserializedDidDocument?.Id);
          //  Assert.NotNull(deseserializedDidDocument?.Context);
            Assert.NotNull(deseserializedDidDocument?.Service);
            Assert.NotNull(reserializedDidDocument);
            Assert.IsType<ServiceExt>(deseserializedDidDocument!.Service![0]);

            Assert.True(JToken.DeepEquals(JToken.Parse(didDocumentFileContents), JToken.Parse(reserializedDidDocument)));
        }


        /// <summary>
        /// This checks plain <see cref="DidDocument"/> deserialization and serialization
        /// succeeds with any valid DID documents.
        /// </summary>
        /// <param name="didDocumentFilename">The DID document data file under test.</param>
        /// <param name="didDocumentFileContents">The DID document data file contents.</param>
        [Theory]
        [FilesData(TestInfrastructureConstants.RelativeTestPath, "*.json", SearchOption.AllDirectories)]
        public void AllTestDIDsAsPlainDocumentsRountrip(string didDocumentFilename, string didDocumentFileContents)
        {
            TestInfrastructureConstants.ThrowIfPreconditionFails(didDocumentFilename, didDocumentFileContents);

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new VerificationRelationshipConverter<IAssertionMethod>(),
                    new VerificationRelationshipConverter<IAuthenticationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityDelegationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityInvocationMethod>(),
                    new VerificationRelationshipConverter<IKeyAgreementMethod>(),
                    new VerificationMethodConverter(),
                    new ServiceConverter(),
                    new JsonLdContextConverter<Context>()
                }
            };

            var deseserializedDidDocument = JsonConvert.DeserializeObject<Json.Newtonsoft.ModelExt.DidDocumentExt>(didDocumentFileContents, settings);
            string reserializedDidDocument = JsonConvert.SerializeObject(deseserializedDidDocument, settings);
            Debug.WriteLine(reserializedDidDocument);

            //All the DID documents need to have an ID and a context.
            Assert.NotNull(deseserializedDidDocument?.Id);
            //Assert.NotNull(deseserializedDidDocument?.Context);
            Assert.NotNull(reserializedDidDocument);

            Assert.True(JToken.DeepEquals(JToken.Parse(didDocumentFileContents), JToken.Parse(reserializedDidDocument)));
        }

        [Theory]
        [FilesData(TestInfrastructureConstants.RelativeTestPath, "did-3 .json", SearchOption.AllDirectories)]
        public void AllTestDIDsAsPlainDocumentsRountrip2(string didDocumentFilename, string didDocumentFileContents)
        {
            TestInfrastructureConstants.ThrowIfPreconditionFails(didDocumentFilename, didDocumentFileContents);

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new VerificationRelationshipConverter<IAssertionMethod>(),
                    new VerificationRelationshipConverter<IAuthenticationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityDelegationMethod>(),
                    new VerificationRelationshipConverter<ICapabilityInvocationMethod>(),
                    new VerificationRelationshipConverter<IKeyAgreementMethod>(),
                    new VerificationMethodConverter(),
                    new ServiceConverter(),
                    new JsonLdContextConverter<Context>()
                }
            };

            var deseserializedDidDocument = JsonConvert.DeserializeObject<Json.Newtonsoft.ModelExt.DidDocumentExt>(didDocumentFileContents, settings);
            string reserializedDidDocument = JsonConvert.SerializeObject(deseserializedDidDocument, settings);
            Debug.WriteLine(reserializedDidDocument);

            //All the DID documents need to have an ID and a context.
            Assert.NotNull(deseserializedDidDocument?.Id);
            //Assert.NotNull(deseserializedDidDocument?.Context);
            Assert.NotNull(reserializedDidDocument);

            Assert.True(JToken.DeepEquals(JToken.Parse(didDocumentFileContents), JToken.Parse(reserializedDidDocument)));
        }
    }




    //TODO: Work in progress.
    //Quickly done some DTOs to test serialization of more specialized services in DidDocumentTests.

    [DebuggerDisplay("OpenIdConnectVersion1(Id = {Id})")]
    public class OpenIdConnectVersion1 : ServiceExt
    { }


    [DebuggerDisplay("SpamCost(Amount = {Amount}, Currency = {Currency})")]
    [DataContract]
    public class SpamCost
    {
        [DataMember(Name = "amount")]
        public string? Amount { get; set; }

        [DataMember(Name = "currency")]
        public string? Currency { get; set; }
    }

    [DebuggerDisplay("SocialWebInboxService(Id = {Id})")]
    [DataContract]
    public class SocialWebInboxService : ServiceExt
    {
        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "spamCost")]
        public SpamCost? SpamCost { get; set; }
    }


    [DebuggerDisplay("VerifiableCredentialService(Id = {Id})")]
    [DataContract]
    public class VerifiableCredentialService : ServiceExt
    {
    }
}
