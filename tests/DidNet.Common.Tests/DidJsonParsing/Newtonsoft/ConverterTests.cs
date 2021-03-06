using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using DidNet.Common.Tests.DidJsonParsing.SystemText;
using DidNet.Json.Newtonsoft.Converters;
using DidNet.Json.SystemText;
using DidNet.Json.SystemText.ModelExt;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace DidNet.Common.Tests.DidJsonParsing.Newtonsoft
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
        private string ComplexContext => @"{ ""@context"": {
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
            var service = JsonConvert.DeserializeObject<ServiceExt>(TestService1);
            Assert.NotNull(service);

            var roundTrippedJson = JsonConvert.SerializeObject(service);
            Assert.NotNull(roundTrippedJson);

            Assert.True(JToken.DeepEquals(JToken.Parse(TestService1), JToken.Parse(roundTrippedJson)));
        }

        

       [Fact]
       public void RoundtripOneUriContext()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new JsonLdContextConverter<Context>()
                }
            };

            var context = JsonConvert.DeserializeObject<IContext>(OneUriContext, settings);
            Assert.NotNull(context);

            var roundTrippedJson = JsonConvert.SerializeObject(context, settings);
            Assert.NotNull(roundTrippedJson);

            Assert.True(JToken.DeepEquals(JToken.Parse(OneUriContext), JToken.Parse(roundTrippedJson)));
        }


        [Fact]
        public void RoundtripCollectionUriContext()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new JsonLdContextConverter<Context>()
                }
            };

            var context = JsonConvert.DeserializeObject<IContext>(CollectionUriContext, settings);
            Assert.NotNull(context);

            var roundTrippedJson = JsonConvert.SerializeObject(context, settings);
            Assert.NotNull(roundTrippedJson);

            Assert.True(JToken.DeepEquals(JToken.Parse(CollectionUriContext), JToken.Parse(roundTrippedJson)));
        }


        [Fact]
        public void RountripComplexContext()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new JsonLdContextConverter<Context>()
                }
            };

            var context = JsonConvert.DeserializeObject<IContext>(ComplexContext, settings);
            Assert.NotNull(context);

            var roundTrippedJson = JsonConvert.SerializeObject(context, settings);
            Assert.NotNull(roundTrippedJson);

            Assert.True(JToken.DeepEquals(JToken.Parse(ComplexContext), JToken.Parse(roundTrippedJson)));
        }
    }
}
