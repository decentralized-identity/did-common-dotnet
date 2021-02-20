using System;

namespace DidNet.Crypto
{
    public interface ICryptoProvider
    {
        /// <summary>
        /// Returns a cryptographic operator that supports the given <paramref name="algorithm"/>.
        /// </summary>
        /// <param name="algorithm">The algorithm to support.</param>
        /// <param name="args">The arguments used to create the cryptographic operator.</param>
        /// <returns>A cryptographic operator supporting the <paramref name="algorithm"/>.</returns>
        object Create(string algorithm, params object[] args);

        bool IsSupportedAlgorithm(string algorithm, params object[] args);

        public void Release(object cryptoInstance);
    }
}
