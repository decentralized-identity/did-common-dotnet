using System;
using DidNet.Common.Verification;

namespace DidNet.Common
{
    public interface IDidDocument: IAdditionalData
    {
        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        IContext? Context { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        Uri? Id { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#control
        /// </summary>
        string[]? Controller { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#verification-methods
        /// </summary>
        IVerificationMethod[]? VerificationMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#authentication
        /// </summary>
        IAuthenticationMethod[]? Authentication { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#assertionmethod
        /// </summary>
        IAssertionMethod[]? AssertionMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#keyagreement
        /// </summary>
        IKeyAgreementMethod[]? KeyAgreement { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilitydelegation
        /// </summary>
        ICapabilityDelegationMethod[]? CapabilityDelegation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilityinvocation
        /// </summary>
        ICapabilityInvocationMethod[]? CapabilityInvocation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#service-endpoints
        /// </summary>
        IService[]? Service { get; set; }

    }
}