using DotDecentralized.Core.Did;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json;
using Xunit;

namespace DotDecentralized.Tests
{
    public class DidDocumentTests
    {
        /// <summary>
        /// An example combining https://www.w3.org/TR/did-core/#example-19-various-service-endpoints and other pieces.
        /// </summary>
        /// <remarks>
        /// Parsing the following service does not function due to ServiceEndpoint currently being a string property instead
        /// of an class. (Remember to refactor also DidDocument.Controller.)
        /// {
        ///   ""id"": ""did:example:123456789abcdefghi#hub"",
        ///        ""type"": ""IdentityHub"",
        ///        ""verificationMethod"": ""did:example:123456789abcdefghi#key-1"",
        ///        ""serviceEndpoint"": {
        ///          ""@context"": ""https://schema.identity.foundation/hub"",
        ///          ""type"": ""UserHubEndpoint"",
        ///          ""instances"": [""did:example:456"", ""did:example:789""]
        /// }}, </remarks>
        private string TestDidDocument1 { get; } = @"{
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
            var originalJson = TestDidDocument1;

            var typeMap = new Dictionary<string, Type>(ServiceConverterFactory.DefaultTypeMap);
            typeMap.Add("OpenIdConnectVersion1.0Service", typeof(OpenIdConnectVersion1));
            typeMap.Add("CredentialRepositoryService", typeof(Service));
            typeMap.Add("XdiService", typeof(Service));
            typeMap.Add("AgentService", typeof(Service));
            typeMap.Add("IdentityHub", typeof(Service));
            typeMap.Add("MessagingService", typeof(Service));
            typeMap.Add("SocialWebInboxService", typeof(SocialWebInboxService));
            typeMap.Add("DidAuthPushModeVersion1", typeof(Service));

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                Converters =
                {
                    new VerificationRelationshipConverterFactory(),
                    new VerificationMethodConverter(),
                    new ServiceConverterFactory(typeMap.ToImmutableDictionary()),
                    new JsonLdContextConverter()
                }
            };

            var didObject = JsonSerializer.Deserialize<DidDocument>(originalJson, options);
            var roundTrippedJson = JsonSerializer.Serialize(didObject, options);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(originalJson);
            using var doc2 = JsonDocument.Parse(roundTrippedJson);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }

        /// <summary>
        /// The reader should be able to deserialize all these test files correctly.
        /// </summary>
        /// <param name="didDocumentFileName">The DID document data file under test.</param>
        /// <param name="didDocumentFileContents">The DID document data file contents.</param>
        [Theory]
        [FilesData(@"..\..\..\TestDocuments", "*.json", SearchOption.AllDirectories)]
        public void SerializationSucceeds(string _, string didDocumentFileContents)
        {
            if(_ == null)
            {
                throw new ArgumentNullException(nameof(_));
            }

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                Converters =
                {
                    new VerificationRelationshipConverterFactory(),
                    new VerificationMethodConverter(),
                    new ServiceConverterFactory(),
                    new JsonLdContextConverter()
                }
            };

            DidDocument? didDocument = JsonSerializer.Deserialize<DidDocument>(didDocumentFileContents, options);
            var roundTrippedDidDocument = JsonSerializer.Serialize(didDocument, options);
            Assert.NotNull(didDocument?.Id);
            Assert.NotNull(roundTrippedDidDocument);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(didDocumentFileContents);
            using var doc2 = JsonDocument.Parse(roundTrippedDidDocument);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }


        /// <summary>
        /// The reader should be able to deserialize all these test files correctly.
        /// </summary>
        /// <param name="didDocumentFileName">The DID document data file under test.</param>
        /// <param name="didDocumentFileContents">The DID document data file contents.</param>
        [Theory]
        [FilesData(@"..\..\..\TestDocuments", "did-1.json", SearchOption.AllDirectories)]
        public void SerializationSucceeds1(string _, string didDocumentFileContents)
        {
            if(_ is null)
            {
                throw new ArgumentNullException(nameof(_));
            }

            var typeMap = new Dictionary<string, Type>(ServiceConverterFactory.DefaultTypeMap);
            typeMap.Add("VerifiableCredentialService", typeof(VerifiableCredentialService));
            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                Converters =
                {
                    new VerificationRelationshipConverterFactory(),
                    new VerificationMethodConverter(),
                    new ServiceConverterFactory(typeMap.ToImmutableDictionary()),
                    new JsonLdContextConverter()
                }
            };

            DidDocument? didDocument = JsonSerializer.Deserialize<DidDocument>(didDocumentFileContents, options);
            var roundTrippedDidDocument = JsonSerializer.Serialize(didDocument, options);
            Assert.NotNull(didDocument?.Id);
            Assert.NotNull(roundTrippedDidDocument);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(didDocumentFileContents);
            using var doc2 = JsonDocument.Parse(roundTrippedDidDocument);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }


        /// <summary>
        /// The reader should be able to deserialize all these test files correctly.
        /// </summary>
        /// <param name="didDocumentFileName">The DID document data file under test.</param>
        /// <param name="didDocumentFileContents">The DID document data file contents.</param>
        [Theory]
        [FilesData(@"..\..\..\TestDocuments", "veresone-1.json", SearchOption.AllDirectories)]
        public void SerializationSucceeds2(string _, string didDocumentFileContents)
        {
            if(_ is null)
            {
                throw new ArgumentNullException(nameof(_));
            }

            var options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                Converters =
                {
                    new VerificationRelationshipConverterFactory(),
                    new VerificationMethodConverter(),
                    new ServiceConverterFactory(),
                    new JsonLdContextConverter()
                }
            };

            DidDocument? didDocument = JsonSerializer.Deserialize<DidDocument>(didDocumentFileContents, options);
            var roundTrippedDidDocument = JsonSerializer.Serialize(didDocument, options);
            Assert.NotNull(didDocument?.Id);
            Assert.NotNull(roundTrippedDidDocument);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(didDocumentFileContents);
            using var doc2 = JsonDocument.Parse(roundTrippedDidDocument);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }
    }
}
