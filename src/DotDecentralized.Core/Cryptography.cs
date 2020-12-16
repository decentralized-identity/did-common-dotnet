using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotDecentralized.Core.Did
{
    /// <summary>
    /// A general purpose wrapper for specific <see cref="SecurityKey"/>
    /// implementations so it can be used in functions defined in this library.
    /// This wrapping is done in <see cref="DidCryptoProvider"/>.
    /// </summary>
    /// <remarks>The libraries that implement the actual algorithms expose their
    /// public and private key material in different ways.
    /// The .NET libraries do not provide a method to expose the
    /// key material in <see cref="SecurityKey"/> or derived implementations.
    /// So a wrapper will be defined that does not enforce inheritance
    /// hierarchy in existing libraries but makes the key material
    /// available for token creation and serialization purposes.</remarks>
    public abstract class AsymmetricKeyWrapper
    {
        /// <summary>
        /// A default constructor for classes that provide the concrete implementation.
        /// </summary>
        /// <param name="signatureProvider">A provider for signatures.</param>
        /// <param name="keyType">The type of key.</param>
        /// <param name="curve">The type of curve.</param>
        protected AsymmetricKeyWrapper(AsymmetricSignatureProvider signatureProvider, string keyType, string curve)
        {
            SignatureProvider = signatureProvider ?? throw new ArgumentNullException(nameof(signatureProvider));
            KeyType = keyType ?? throw new ArgumentNullException(nameof(keyType));
            Curve = curve ?? throw new ArgumentNullException(nameof(curve));
        }

        /// <summary>
        /// The curve used by the wrapped key.
        /// </summary>
        public string Curve { get; }

        /// <summary>
        /// The key type used by the wrapped key.
        /// </summary>
        public string KeyType { get; }

        /// <summary>
        /// A provider for signatures.
        /// </summary>
        public AsymmetricSignatureProvider SignatureProvider { get; }

        /// <summary>
        /// The status of private key in the type wrapped.
        /// </summary>
        public abstract PrivateKeyStatus PrivateKeyStatus { get; }

        /// <summary>
        /// Public key octets as defined in <see href="https://tools.ietf.org/html/rfc7517"/> if exists. Otherwise <see cref="Array.Empty{byte}"/>.
        /// </summary>
        public abstract byte[] PublicKeyBytes { get; }

        /// <summary>
        /// If accessible, private key octets as defined in <see href="https://tools.ietf.org/html/rfc7517"/> if <see cref="PrivateKeyStatus"/> is <see cref="PrivateKeyStatus.Exists"/>.
        /// Otherwise <see cref="Array.Empty{byte}"/>.
        /// </summary>
        public abstract byte[] PrivateKeyBytes { get; }
    }


    /// <summary>
    /// A purpose built wrapper for Bouncy Castle specific provider implemented within this library.
    /// </summary>
    /// <remarks>A wrapper like this is needed for any specific crytographic library.</remarks>
    public class BouncyCastleEdDsaWrapper: AsymmetricKeyWrapper
    {
        /// <summary>
        /// The key material.
        /// </summary>
        private BouncyCastleEdDsaSecurityKey SecurityKey { get; }

        /// <summary>
        /// Constructs BouncyCastleEdDsaWrapper.
        /// </summary>
        /// <param name="signatureProvider">A provider for signatures.</param>
        /// <param name="curve">The type of curve.</param>
        /// <param name="securityKey">The security key containing the key material.</param>
        public BouncyCastleEdDsaWrapper(AsymmetricSignatureProvider signatureProvider, BouncyCastleEdDsaSecurityKey securityKey, string curve): base(signatureProvider, CryptographyAlgorithmConstants.EdDsa.KeyType, curve)
        {
            SecurityKey = securityKey ?? throw new ArgumentNullException(nameof(securityKey));
        }

        /// <inheritdoc/>
        public override PrivateKeyStatus PrivateKeyStatus => SecurityKey.PrivateKeyStatus;

        /// <inheritdoc/>
        public override byte[] PublicKeyBytes => SecurityKey.PublicKeyBytes;

        /// <inheritdoc/>
        public override byte[] PrivateKeyBytes => SecurityKey.PrivateKeyBytes;
    }




    //TODO: Summary copied from https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/dev/src/Microsoft.IdentityModel.Tokens/JsonWebKeyConverter.cs,
    //edit as needed but keep close enough to the original. See the other overloads in this class that could be overloaded?
    /// <summary>
    /// Converts a <see cref="SecurityKey"/> into a <see cref="JsonWebKey"/>.
    /// Supports: converting to a <see cref="JsonWebKey"/> from one of: <see cref="RsaSecurityKey"/>, <see cref="X509SecurityKey"/>, and <see cref=" SymmetricSecurityKey"/>.
    /// </summary>
    public class DidWebKeyConverter: JsonWebKeyConverter
    {
        /// <summary>
        /// Converts a <see cref="AsymmetricKeyMaterialWrapper"/> into a <see cref="JsonWebKey"/>.
        /// </summary>
        /// <param name="key">a <see cref="RsaSecurityKey"/> to convert.</param>
        /// <returns>a <see cref="JsonWebKey"/></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="key"/>is null.</exception>
        public static JsonWebKey ConvertFromEdDsaSecurityKey(AsymmetricKeyWrapper key)
        {
            if(key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return new JsonWebKey
            {
                D = key.PrivateKeyStatus == PrivateKeyStatus.Exists ? Base64UrlEncoder.Encode(key.PrivateKeyBytes) : null,
                X = Base64UrlEncoder.Encode(key.PublicKeyBytes),
                Kty = key.KeyType,
                Crv = key.Curve
            };
        }
    }


    /// <summary>
    /// A convenience loader to find <see cref="ICryptoProvider"/> implementations.
    /// </summary>
    public static class CryptoProviderLoader
    {
        /// <summary>
        /// A convenience function to load all <see cref="ICryptoProvider"/> types
        /// except <see cref="DidCryptoProvider"/>.
        /// </summary>
        /// <returns></returns>
        public static IList<ICryptoProvider> LoadCryptoProviders()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ICryptoProvider).IsAssignableFrom(type) && type.IsClass && type != typeof(DidCryptoProvider))
                .Select(p => (ICryptoProvider)Activator.CreateInstance(p)!)
                .ToList();
        }
    }

    /// <summary>
    /// This is a purpose built loader for <see cref="ICryptoProvider"/>
    /// libraries built in to this library. If others are used, this
    /// provider needs to be extended or refactored. The gist of the matter
    /// is to know in which format to provide the parameters to respective
    /// <see cref="ICryptoProvider"/> types, what types are the return values
    /// and how to wrap them so their key material is exposed appropriately.
    /// See <seealso href="https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Using-a-custom-CryptoProvider"/>
    /// and <seealso href="https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/bbf721fedca5dabc0d77e22828a5948be0d3fa86/test/Microsoft.IdentityModel.Tokens.Tests/CustomCryptoProviders.cs"/>.
    /// </summary>
    public class DidCryptoProvider: ICryptoProvider
    {
        private IList<ICryptoProvider> CryptoProviders { get; }


        public DidCryptoProvider(IEnumerable<ICryptoProvider> cryptoProviders)
        {
            CryptoProviders = new List<ICryptoProvider>(cryptoProviders);
        }


        public object Create(string algorithm, params object[] args)
        {
            //This provider should know hot to transform args so they are acceptable
            //to the specific loaded ICryptoProvider implementations.
            var jwkObj = args[0] as JsonWebKey;
            if(jwkObj != null)
            {
                foreach(var cryptoProvider in CryptoProviders)
                {
                    if(cryptoProvider is BouncyCastleCryptoProvider)
                    {
                        cryptoProvider.IsSupportedAlgorithm(algorithm, args);
                        var signatureProvider = (AsymmetricSignatureProvider)cryptoProvider.Create(algorithm, args);
                        var key = (BouncyCastleEdDsaSecurityKey)signatureProvider.Key;
                        return new BouncyCastleEdDsaWrapper(signatureProvider, key, jwkObj.Crv);
                    }
                }
            }

            throw new NotSupportedException();
        }


        /// <inheritdoc />
        public bool IsSupportedAlgorithm(string algorithm, params object[] args)
        {
            foreach(var cryptoProvider in CryptoProviders)
            {
                if(cryptoProvider.IsSupportedAlgorithm(algorithm, args))
                {
                    return true;
                }
            }

            return false;
        }


        /// <inheritdoc />
        public void Release(object cryptoInstance)
        {
            if(cryptoInstance is IDisposable disposableObject)
            {
                disposableObject.Dispose();
            }
        }
    }
}
