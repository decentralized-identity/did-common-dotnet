using DotDecentralized.BouncyCastle;
using DotDecentralized.Core.Did;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotDecentralized.Tests
{
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
        public BouncyCastleEdDsaWrapper(AsymmetricSignatureProvider signatureProvider, BouncyCastleEdDsaSecurityKey securityKey, string curve):
            base(signatureProvider, CryptographyAlgorithmConstants.EdDsa.KeyType, curve)
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


    /// <summary>
    /// A convenience loader to find <see cref="ICryptoProvider"/> implementations.
    /// </summary>
    public static class CryptoProviderLoader
    {
        /// <summary>
        /// A convenience function to load all <see cref="ICryptoProvider"/> types
        /// exept <see cref="DidCryptoProvider"/>.
        /// </summary>
        /// <returns>A collection of <see cref="ICryptoProvider"/> implementations.</returns>
        public static IReadOnlyCollection<ICryptoProvider> LoadCryptoProviders()
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
    /// libraries built into this library. If others are used, this
    /// provider needs to be extended or refactored. The gist of the matter
    /// is to know in which format to provide the parameters to respective
    /// <see cref="ICryptoProvider"/> types, what types are the return values
    /// and how to wrap them so their key material is exposed appropriately.
    /// See <seealso href="https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Using-a-custom-CryptoProvider"/>
    /// and <seealso href="https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/bbf721fedca5dabc0d77e22828a5948be0d3fa86/test/Microsoft.IdentityModel.Tokens.Tests/CustomCryptoProviders.cs"/>.
    /// </summary>
    public class DidCryptoProvider: ICryptoProvider
    {
        /// <summary>
        /// A list of <see cref="ICryptoProvider"/> implementations that the
        /// library can use.
        /// </summary>
        private IReadOnlyCollection<ICryptoProvider> CryptoProviders { get; }


        /// <summary>
        /// A list of <see cref="ICryptoProvider"/> implementations that the library can use.
        /// </summary>
        /// <param name="cryptoProviders"></param>
        public DidCryptoProvider(IReadOnlyCollection<ICryptoProvider> cryptoProviders)
        {
            CryptoProviders = cryptoProviders;
        }


        //TODO: See the stated .NET interface expectation to release objects once used. Should be cached or something since
        //loading by reflection and giving instances via a constructor does not work with this.
        /// <inheritdoc/>
        public object Create(string algorithm, params object[] args)
        {
            //This provider should know how to transform args so they are acceptable
            //to the specific loaded ICryptoProvider implementations. Ideally the provider
            //needs not to know DotDecentralized specific types but could be an independent
            //provider.
            //
            //OBS! This provider of course can be used to transform the parameters to the type
            //the provider needs them. So, this type is also an adapter.
            //See also the class level comments.
            var jwkObj = args[0] as JsonWebKey;
            if(jwkObj != null)
            {
                foreach(var cryptoProvider in CryptoProviders)
                {
                    if(cryptoProvider is BouncyCastleCryptoProvider)
                    {
                        //TODO: When BouncyCastle supports more algorithms and curves, they should be checked here
                        //and not assumed the return type is a EdDsaWrapper.
                        cryptoProvider.IsSupportedAlgorithm(algorithm, args);
                        var signatureProvider = (AsymmetricSignatureProvider)cryptoProvider.Create(algorithm, args);

                        //The Microsoft libraries do not provide types to expose private key material nor

                        //so the types are
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
