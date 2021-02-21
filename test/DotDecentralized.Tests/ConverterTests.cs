using DotDecentralized.Core.Did;
using System.Text.Json;
using Xunit;

namespace DotDecentralized.Tests
{
    /// <summary>
    /// Tests for individual converters
    /// </summary>
    public class ConverterTests
    {
        /// <summary>
        /// A sample test service copied from https://www.w3.org/TR/did-core/.
        /// </summary>
        private string TestService1 => @"{
                ""id"": ""did:example:123456789abcdefghi#oidc"",
                ""type"": ""OpenIdConnectVersion1.0Service"",
                ""serviceEndpoint"": ""https://openid.example.com/"" }";

        /// <summary>
        /// The DID Uri from https://www.w3.org/TR/did-core/.
        /// </summary>
        private string OneUriContext => @"{""@context"": ""https://www.w3.org/ns/did/v1""}";

        /// <summary>
        /// A collection of URIs in context.
        /// </summary>
        private string CollectionUriContext => @"{""@context"": [""https://w3id.org/future-method/v1"", ""https://w3id.org/veres-one/v1""]}";

        /// <summary>
        /// A sample complex @context copied from https://json-ld.org/playground/ JSON-LD 1.1 compacted Place sample.
        /// </summary>
        private string ComplexContext1 => @"{ ""@context"": {
            ""name"": ""http://schema.org/name"",
            ""description"": ""http://schema.org/description"",
            ""image"": {
                ""@id"": ""http://schema.org/image"",
                ""@type"": ""@id""
            },
            ""geo"": ""http://schema.org/geo"",
            ""latitude"": {
                ""@id"": ""http://schema.org/latitude"",
                ""@type"": ""xsd:float""
              },
           ""longitude"": {
                ""@id"": ""http://schema.org/longitude"",
                ""@type"": ""xsd:float""
            },
           ""xsd"": ""http://www.w3.org/2001/XMLSchema#""
           },
           ""name"": ""The Empire State Building"",
           ""description"": ""The Empire State Building is a 102-story landmark in New York City."",
           ""image"": ""http://www.civil.usherbrooke.ca/cours/gci215a/empire-state-building.jpg"",
           ""geo"": {
              ""latitude"": ""40.75"",
              ""longitude"": ""73.98""
              }
            }";


        [Fact]
        public void RoundtripServiceTest1()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ServiceConverterFactory());

            Service? service = JsonSerializer.Deserialize<Service>(TestService1, options);
            Assert.NotNull(service);

            var roundTrippedJson = JsonSerializer.Serialize(service, options);
            Assert.NotNull(roundTrippedJson);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(TestService1);
            using var doc2 = JsonDocument.Parse(roundTrippedJson);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }


        [Fact]
        public void RoundtripOneUriContext()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonLdContextConverter());

            Context? context = JsonSerializer.Deserialize<Context>(OneUriContext, options);
            Assert.NotNull(context);

            var roundTrippedJson = JsonSerializer.Serialize(context, options);
            Assert.NotNull(roundTrippedJson);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(OneUriContext);
            using var doc2 = JsonDocument.Parse(roundTrippedJson);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }


        [Fact]
        public void RoundtripCollectionUriContext()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonLdContextConverter());

            Context? context = JsonSerializer.Deserialize<Context>(CollectionUriContext, options);
            Assert.NotNull(context);

            var roundTrippedJson = JsonSerializer.Serialize(context, options);
            Assert.NotNull(roundTrippedJson);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(CollectionUriContext);
            using var doc2 = JsonDocument.Parse(roundTrippedJson);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }


        [Fact]
        public void RountripComplexContext1()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonLdContextConverter());

            Context? context = JsonSerializer.Deserialize<Context>(ComplexContext1, options);
            Assert.NotNull(context);

            var roundTrippedJson = JsonSerializer.Serialize(context, options);
            Assert.NotNull(roundTrippedJson);

            var comparer = new JsonElementComparer();
            using var doc1 = JsonDocument.Parse(ComplexContext1);
            using var doc2 = JsonDocument.Parse(roundTrippedJson);
            Assert.True(comparer.Equals(doc1.RootElement, doc2.RootElement));
        }
    }
}
