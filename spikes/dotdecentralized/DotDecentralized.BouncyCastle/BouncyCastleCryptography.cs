using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System;

namespace DotDecentralized.BouncyCastle
{
    //TODO: It sould be decided what is the parameter format that goes into BouncyCastleCryptoProvider.
    //In general in test there is a sample of DidCryptoProvider: ICryptoProvider that mimicks application
    //specific loader that is an adapter to any custom implementations such as BouncyCastleCryptoProvider
    //and so can transform parameters to any library specific format. But as this is a DID Core library
    //specific implementation we control, we also can decide the parameter format.
    //
    //Preferably the format would be something that depends only on .NET platform, but there really
    //isn't a widely accepted format.
    //
    //Also as for the return value format, DidCryptoProvider can wrap it to appropriate type that
    //the other elements in the library can use and depend on.


    /// <summary>
    /// Defines a BouncyCastle specific <see cref="ICryptoProvider"/> implementation.
    /// See <seealso href="https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Using-a-custom-CryptoProvider"/>.
    /// </summary>
    public class BouncyCastleCryptoProvider: ICryptoProvider
    {
        /// <summary>
        /// Returns a cryptographic operator that supports the given <paramref name="algorithm"/>.
        /// </summary>
        /// <param name="algorithm">The algorithm to support.</param>
        /// <param name="args">The arguments used to create the cryptographic operator.</param>
        /// <returns>A cryptographic operator supporting the <paramref name="algorithm"/>.</returns>
        public object Create(string algorithm, params object[] args)
        {
            if(IsSupportedAlgorithm(algorithm, args))
            {
                //TODO: Support more curves.
                if(algorithm.Equals("EdDSA", StringComparison.OrdinalIgnoreCase))
                {
                    var keyMaterial = args[0] as JsonWebKey;
                    if(keyMaterial != null)
                    {
                        //TODO: Should this allowed even if it increase hazard potential?
                        if(keyMaterial.X != null && keyMaterial.Y != null)
                        {
                            throw new ArgumentException("Currently it is not allowed to create a signature provider that supports both signing and verifying at the same time.");
                        }

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
                            throw new ArgumentException("Key material needs to be provided.");
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
        /// <summary>
        /// A constructor for BouncyCastle specific asymmetric key material
        /// that connects it to .NET framework and DotDecentralized
        /// cryptographic facilities.
        /// </summary>
        /// <param name="keyParameter">The key material.</param>
        /// <param name="curve">The curve this key material supports.</param>
        /// <param name="cryptoProvider">The cryptographic provider this key material is tied to.</param>
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
        public override int KeySize => 32;


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
    /// Signature and verification provider implemented by BouncyCastle.
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
        public BouncyCastleEdDsaSignatureProvider(BouncyCastleEdDsaSecurityKey key, string algorithm):
            base(key, algorithm, willCreateSignatures: key.PrivateKeyStatus == PrivateKeyStatus.Exists)
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

            if(!WillCreateSignatures)
            {
                throw new InvalidOperationException("Private key is needed to create signatures.");
            }

            //TODO: Support more curves.
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

            //TODO: Support more curves.
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
