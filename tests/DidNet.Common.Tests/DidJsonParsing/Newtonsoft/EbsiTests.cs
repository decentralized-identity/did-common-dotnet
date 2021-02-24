using System.Collections;
using DidNet.Common.Tests.DidJsonParsing.SystemText;
using DidNet.Common.Verification;
using DidNet.Json.Newtonsoft.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace DidNet.Common.Tests.DidJsonParsing.Newtonsoft
{
    
    /// <summary>
    /// EBSI specific tests. The source is at <a href="https://api.ebsi.xyz/docs/?urls.primaryName=DID%20API#/DID/get-did-v1-identifier">EBSI DID API Swagger</a>.
    /// </summary>
    public class EbsiDidTests
    {
        /// <summary>
        /// The reader should be able to deserialize all these test files correctly.
        /// </summary>
        /// <param name="didDocumentFileName">The DID document data file under test.</param>
        /// <param name="didDocumentFileContents">The DID document data file contents.</param>
        [Theory]
        [FilesData(TestInfrastructureConstants.RelativeTestPath + "EBSI", "ebsi-did-1.json")]
        public void SerializationSucceeds2(string didDocumentFilename, string didDocumentFileContents)
        {
            TestInfrastructureConstants.ThrowIfPreconditionFails(didDocumentFilename, didDocumentFileContents);
            
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
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

            //All the DID documents need to have an ID and a context. This one needs to have also a strongly type element.
            Assert.NotNull(deseserializedDidDocument?.Id);
           // Assert.NotNull(deserialize?.Context);
            Assert.NotNull(reserializedDidDocument);

            
            //Assert.Single(deseserializedDidDocument?.AdditionalData as IEnumerable);
            Assert.IsType<JArray>(deseserializedDidDocument!.AdditionalData!["publicKey"]);

            Assert.True(JToken.DeepEquals(JToken.Parse(didDocumentFileContents), JToken.Parse(reserializedDidDocument)));
        }
    }
}
