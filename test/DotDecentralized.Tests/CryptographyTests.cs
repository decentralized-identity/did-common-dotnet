using DotDecentralized.Core;
using DotDecentralized.Core.Did;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;
using Xunit;

namespace DotDecentralized.Tests
{
    public class CryptographyTests
    {
        /// <summary>
        /// A temporary test to check key material loading.
        /// </summary>
        [Fact]
        public void BouncyCastleCryptoProviderTest1()
        {
            var cryptoProviders = CryptoProviderLoader.LoadCryptoProviders();
            var didCryptoProvider = new DidCryptoProvider(cryptoProviders);

            //Copied from did-3.json...
            const string Jwk = @"{ ""kty"": ""OKP"", ""crv"": ""X25519"", ""x"": ""OeXe54Y0Dnk0WNWsQ6PqKUBB2x6bos0DZ_WkdFNdt3M"" }";
            var jwkObj = JsonWebKey.Create(Jwk);
            var algorithm = CryptographyAlgorithmConstants.EdDsa.Algorithm;

            Assert.True(didCryptoProvider.IsSupportedAlgorithm(algorithm, jwkObj), "Unsupported algorithm was tried.");

            var keyWrapper = didCryptoProvider.Create(algorithm, jwkObj);
            Assert.NotNull(keyWrapper);
            Assert.True(keyWrapper is AsymmetricKeyWrapper);
            Assert.True(keyWrapper is BouncyCastleEdDsaWrapper);
        }


        /// <summary>
        /// A test for roundtripping signing and verifying...
        /// </summary>
        [Fact]
        public void BouncyCastleCryptoProviderTest2()
        {
            //TODO: Might be useful to provide seed bytes to SignatureProvider and use that to rountrip?
            byte[] seedBytes = new byte[] { 0x1 };
            //var randomnessGenerator = SecureRandom.GetInstance("SHA1PRNG", false);
            //randomnessGenerator.SetSeed(seedBytes);
            var randomessGenerator = new SecureRandom();

            var keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(randomessGenerator));
            var keyPair = keyPairGenerator.GenerateKeyPair();

            var cryptoProvider = new BouncyCastleCryptoProvider();
            const string Curve = CryptographyAlgorithmConstants.EdDsa.Curves.Ed25519;
            var privateKey = new BouncyCastleEdDsaSecurityKey(keyPair.Private, Curve, cryptoProvider);
            var publicKey = new BouncyCastleEdDsaSecurityKey(keyPair.Public, Curve, cryptoProvider);

            const string Algorithm = CryptographyAlgorithmConstants.EdDsa.Algorithm;
            var privateSignatureProvider = new BouncyCastleEdDsaSignatureProvider(privateKey, Algorithm);
            var publicSignatureProvider = new BouncyCastleEdDsaSignatureProvider(publicKey, Algorithm);

            var testBytes = Encoding.UTF8.GetBytes("test");
            var signedTestBytes = privateSignatureProvider.Sign(testBytes);
            Assert.True(publicSignatureProvider.Verify(testBytes, signedTestBytes));
        }
    }
}
