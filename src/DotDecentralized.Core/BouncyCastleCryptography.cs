using DotDecentralized.Core.Did;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System;

namespace DotDecentralized.Core
{
    /// <summary>
    /// Defines a BouncyCastle specific <see cref="ICryptoProvider"/> implementation.
    /// See <seealso href="https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Using-a-custom-CryptoProvider"/>.
    /// </summary
    public class BouncyCastleCryptoProvider: ICryptoProvider
    {
        /// <inheritdoc/>
        public object Create(string algorithm, params object[] args)
        {
            if(IsSupportedAlgorithm(algorithm, args))
            {
                if(algorithm.Equals("EdDSA", StringComparison.OrdinalIgnoreCase))
                {
                    var keyMaterial = args[0] as JsonWebKey;
                    if(keyMaterial != null)
                    {
                        //TODO: Probably should check a case where both are defined (or some other combinations).
                        AsymmetricKeyParameter? keyParameter = null;
                        if(keyMaterial.X != null)
                        {
                            var decodedPublicBytes = Base64UrlEncoder.DecodeBytes(keyMaterial.X);
                            keyParameter = new Ed25519PublicKeyParameters(decodedPublicBytes, 0);
                        }
                        else if(keyMaterial.D != null)
                        {
                            var decodedPrivateBytes = Base64UrlEncoder.DecodeBytes(keyMaterial.D);
                            keyParameter = new Ed25519PrivateKeyParameters(decodedPrivateBytes, 0);
                        }
                        else
                        {
                            throw new ArgumentException("Key material needs to be provided");
                        }

                        var securityKey = new BouncyCastleEdDsaSecurityKey(keyParameter, keyMaterial.Crv, this);
                        return new BouncyCastleEdDsaSignatureProvider(securityKey, algorithm);
                    }

                    throw new ArgumentException($"The key material argument in position args[0] expected is \"{typeof(JsonWebKey)}\".");
                }
            }

            throw new NotSupportedException();
        }


        /// <inheritdoc/>
        public bool IsSupportedAlgorithm(string algorithm, params object[] args)
        {
            //Currently only this is one is supported, all possible DID Core specific ones should be provided.
            return algorithm.Equals("EdDSA", StringComparison.OrdinalIgnoreCase);
        }


        /// <inheritdoc/>
        public void Release(object cryptoInstance)
        {
            if(cryptoInstance is SignatureProvider signatureProvider)
            {
                //TODO: Not implemented yet.
                //cache.TryRemove(signatureProvider);
            }

            if(cryptoInstance is IDisposable obj)
            {
                obj.Dispose();
            }
        }
    }


    /// <summary>
    /// A Bouncy Castle specific class for a EdDsa key material.
    /// See <seealso href="https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Using-a-custom-CryptoProvider"/>.
    /// </summary>
    public class BouncyCastleEdDsaSecurityKey: AsymmetricSecurityKey
    {
        public BouncyCastleEdDsaSecurityKey(AsymmetricKeyParameter keyParameter, string curve, ICryptoProvider cryptoProvider)
        {
            CryptoProviderFactory.CustomCryptoProvider = cryptoProvider ?? new BouncyCastleCryptoProvider();
            KeyParameter = keyParameter ?? throw new ArgumentNullException(nameof(keyParameter));
            Curve = curve ?? throw new ArgumentNullException(nameof(curve));
        }

        /// <summary>
        /// The EdDSA curve this key uses.
        /// </summary>
        public string Curve { get; }

        /// <summary>
        /// Bouncy Castle specific key parameters. Used by <see cref="BouncyCastleEdDsaSignatureProvider"/>.
        /// </summary>
        internal AsymmetricKeyParameter KeyParameter { get; }

        //Copied from
        //https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/ffa4e55e101978a27059ed8cf854388bc85450f3/src/Microsoft.IdentityModel.Tokens/ECDsaSecurityKey.cs#L66
        [Obsolete("HasPrivateKey method is deprecated, please use FoundPrivateKey instead.")]
        public override bool HasPrivateKey => KeyParameter.IsPrivate;

        /// <inheritdoc />
        public override PrivateKeyStatus PrivateKeyStatus => KeyParameter.IsPrivate ? PrivateKeyStatus.Exists : PrivateKeyStatus.DoesNotExist;

        /// <inheritdoc />
        public override int KeySize => CryptographyAlgorithmConstants.EdDsa.KeySizeInBytes;


        /// <inheritdoc/>
        public override bool IsSupportedAlgorithm(string algorithm)
        {
            return CryptoProviderFactory?.IsSupportedAlgorithm(algorithm) == true;
        }

        /// <summary>
        /// The public key bytes of this security key if they exist.
        /// </summary>
        public byte[] PublicKeyBytes { get { return ((Ed25519PublicKeyParameters)KeyParameter).GetEncoded(); } }

        /// <summary>
        /// The private key bytes of this security key if they exist and are accessible.
        /// </summary>
        public byte[] PrivateKeyBytes { get { return ((Ed25519PrivateKeyParameters)KeyParameter).GetEncoded(); } }
    }


    //Signature provider has the functionality to create a signature for given bytes and
    //validate a signature.
    //TODO: See about validators: https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/ValidatingTokens.

    /// <summary>
    /// Bla.
    /// <see href = "https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/ffa4e55e101978a27059ed8cf854388bc85450f3/src/Microsoft.IdentityModel.Tokens/AsymmetricSignatureProvider.cs" />.
    /// </summary>
    public class BouncyCastleEdDsaSignatureProvider: AsymmetricSignatureProvider
    {
        /// <summary>
        /// The key material this signer uses.
        /// </summary>
        private BouncyCastleEdDsaSecurityKey EdDsaKey { get; }

        /// <summary>
        /// Default constructor for this signer.
        /// </summary>
        /// <param name="key">The key material.</param>
        /// <param name="algorithm">The algorithm.</param>
        public BouncyCastleEdDsaSignatureProvider(BouncyCastleEdDsaSecurityKey key, string algorithm): base(key, algorithm, willCreateSignatures: key.PrivateKeyStatus == PrivateKeyStatus.Exists)
        {
            EdDsaKey = key;
        }


        /// <inheritdoc/>
        public override byte[] Sign(byte[] input)
        {
            if(input == null || input.Length == 0)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if(EdDsaKey.Curve.Equals("Ed25519"))
            {
                var privateKey = (Ed25519PrivateKeyParameters)EdDsaKey.KeyParameter;
                var signer = new Ed25519Signer();
                signer.Init(forSigning: true, privateKey);
                signer.BlockUpdate(input, off: 0, len: input.Length);

                return signer.GenerateSignature();
            }

            throw new ArgumentException($"Unsupported EdDSA curve: \"{EdDsaKey.Curve}\".");
        }


        /// <inheritdoc/>
        public override bool Verify(byte[] input, byte[] signature)
        {
            if(input == null || input.Length == 0)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if(signature == null || signature.Length == 0)
            {
                throw new ArgumentNullException(nameof(signature));
            }

            if(EdDsaKey.Curve.Equals("Ed25519"))
            {
                var publicKey = (Ed25519PublicKeyParameters)EdDsaKey.KeyParameter;
                var validator = new Ed25519Signer();
                validator.Init(forSigning: false, publicKey);
                validator.BlockUpdate(input, off: 0, len: input.Length);

                return validator.VerifySignature(signature);
            }

            throw new ArgumentException($"Unsupported EdDSA curve: \"{EdDsaKey.Curve}\".");
        }
    }
}
