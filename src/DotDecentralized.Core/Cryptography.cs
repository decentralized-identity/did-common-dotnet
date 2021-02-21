using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotDecentralized.Core.Did
{
    //These are program specific adapters needed when one wants to load
    //cryptographic provider specific libraries without DID Core types
    //taking a dependency on specific providers.

    /// <summary>
    /// A general purpose wrapper for specific <see cref="SecurityKey"/>
    /// and <see cref="AsymmetricSecurityKey"/>
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
        /// Otherwise implementation specific.
        /// </summary>
        public abstract byte[] PrivateKeyBytes { get; }
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
                X = Base64UrlEncoder.Encode(key.PublicKeyBytes),
                D = key.PrivateKeyStatus == PrivateKeyStatus.Exists ? Base64UrlEncoder.Encode(key.PrivateKeyBytes) : null,
                Kty = key.KeyType,
                Crv = key.Curve
            };
        }
    }
}
