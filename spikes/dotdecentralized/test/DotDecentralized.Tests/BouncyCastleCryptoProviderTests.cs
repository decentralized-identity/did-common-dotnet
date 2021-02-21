using DotDecentralized.BouncyCastle;
using DotDecentralized.Core;
using DotDecentralized.Core.Did;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Xunit;

namespace DotDecentralized.Tests
{
    /// <summary>
    /// These test specifically Bouncy Castle as the cryptographic provider.
    /// </summary>
    public class BouncyCastleCryptoProviderTests
    {
        /// <summary>
        /// A temporary test to check key material loading.
        /// </summary>
        [Fact]
        public void CanCreateEd25519Verifier()
        {
            const string Algorithm = CryptographyAlgorithmConstants.EdDsa.Algorithm;
            const string Curve = CryptographyAlgorithmConstants.EdDsa.Curves.Ed25519;
            const string KeyType = CryptographyAlgorithmConstants.EdDsa.KeyType;
            var publicPrivateJwk = CryptoUtilities.GeneratePublicPrivateJwk(KeyType, Curve, new byte[] { 0x1 });

            //TODO: Change parameter format to the factory to something else than JWK.
            //The private key needs to be removed as the factory should create a verifier.
            var publicJwk = new JsonWebKey(publicPrivateJwk);
            publicJwk.D = null;
            var cryptoProvider = new BouncyCastleCryptoProvider();
            var bouncyCastleSignatureVerifier = (AsymmetricSignatureProvider)cryptoProvider.Create(Algorithm, publicJwk);

            Assert.False(bouncyCastleSignatureVerifier.WillCreateSignatures, "This provider should not be able to create signatures.");
            Assert.IsAssignableFrom<BouncyCastleEdDsaSignatureProvider>(bouncyCastleSignatureVerifier);
        }


        [Fact]
        public void CanCreateEd25519Signer()
        {
            const string Algorithm = CryptographyAlgorithmConstants.EdDsa.Algorithm;
            const string Curve = CryptographyAlgorithmConstants.EdDsa.Curves.Ed25519;
            const string KeyType = CryptographyAlgorithmConstants.EdDsa.KeyType;
            var publicPrivateJwk = CryptoUtilities.GeneratePublicPrivateJwk(KeyType, Curve, new byte[] { 0x1 });

            //TODO: Change parameter format to the factory to something else than JWK.
            //The public key needs to be removed as the factory should create a signer.
            var privateJwk = new JsonWebKey(publicPrivateJwk);
            privateJwk.X = null;
            var cryptoProvider = new BouncyCastleCryptoProvider();
            var bouncyCastleSignatureProvider = (AsymmetricSignatureProvider)cryptoProvider.Create(Algorithm, privateJwk);

            Assert.True(bouncyCastleSignatureProvider.WillCreateSignatures, "This provider should be able to create signatures.");
            Assert.IsAssignableFrom<BouncyCastleEdDsaSignatureProvider>(bouncyCastleSignatureProvider);
        }


        [Fact]
        public void CanSignAndVerifyEd25519()
        {
            const string Algorithm = CryptographyAlgorithmConstants.EdDsa.Algorithm;
            const string Curve = CryptographyAlgorithmConstants.EdDsa.Curves.Ed25519;
            const string KeyType = CryptographyAlgorithmConstants.EdDsa.KeyType;
            var publicPrivateJwk = CryptoUtilities.GeneratePublicPrivateJwk(KeyType, Curve, new byte[] { 0x1 });

            var publicJwk = new JsonWebKey(publicPrivateJwk);
            var privateJwk = new JsonWebKey(publicPrivateJwk);
            publicJwk.D = null;
            privateJwk.X = null;

            var cryptoProvider = new BouncyCastleCryptoProvider();
            var bouncyCastleSignatureVerifier = (AsymmetricSignatureProvider)cryptoProvider.Create(Algorithm, publicJwk);
            var bouncyCastleSignatureProvider = (AsymmetricSignatureProvider)cryptoProvider.Create(Algorithm, privateJwk);

            var testBytes = Encoding.UTF8.GetBytes("This string is the soure of some test bytes for BouncyCastle types.");
            var signedTestBytes = bouncyCastleSignatureProvider.Sign(testBytes);
            Assert.True(bouncyCastleSignatureVerifier.Verify(testBytes, signedTestBytes));
        }


        public static byte[] HexToByte(string hexString)
        {
            byte[] byArray = new byte[hexString.Length / 2];
            for(int i = 0; i < byArray.Length; i++)
            {
                byArray[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return byArray;
        }
    }
}
