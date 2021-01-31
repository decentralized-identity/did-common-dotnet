using DotDecentralized.BouncyCastle;
using DotDecentralized.Core.Did;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DotDecentralized.Tests
{
    /// <summary>
    /// Cryptographic utilities used in testing.
    /// </summary>
    public static class CryptoUtilities
    {
        /// <summary>
        /// Creates a JWK string with valid public and private key information.
        /// </summary>
        /// <param name="keyType">The JWK key type.</param>
        /// <param name="curve">The JWK curve type.</param>
        /// <param name="seed">The seed for the public private key type.</param>
        /// <returns>A JWK string with public and private key type information.</returns>
        [return: NotNull]
        public static string GeneratePublicPrivateJwk(string keyType, string curve, byte[] seed)
        {
            if(!curve.Equals(CryptographyAlgorithmConstants.EdDsa.Curves.Ed25519))
            {
                throw new ArgumentException("Currently only Ed25519 supported. Fix when more implementd.");
            }

            //OBS! BouncyCastle is used here for convenience to generate the key bytes.
            //This should be provider independent, but in practice a library can sometimes
            //use encoding specific to it that is not recognized other libraries. Pay attention!
            var randomnessGenerator = SecureRandom.GetInstance("SHA256PRNG", autoSeed: false);
            randomnessGenerator.SetSeed(seed);

            var keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(randomnessGenerator));
            var keyPair = keyPairGenerator.GenerateKeyPair();

            var publicKeyBytes = ((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded();
            var privateKeyBytes = ((Ed25519PrivateKeyParameters)keyPair.Private).GetEncoded();
            var cryptoProvider = new BouncyCastleCryptoProvider();

            var publicKeyBytesInBase64 = Base64UrlEncoder.Encode(publicKeyBytes);
            var privateKeyBytesInBase64 = Base64UrlEncoder.Encode(privateKeyBytes);

            return $@"{{ ""kty"": ""{keyType}"", ""crv"": ""{curve}"", ""x"": ""{publicKeyBytesInBase64}"", ""d"": ""{privateKeyBytesInBase64}"" }}";
        }
    }
}
