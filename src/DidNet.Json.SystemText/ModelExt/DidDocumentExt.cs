using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using DidNet.Common;
using DidNet.Common.Verification;

namespace DidNet.Json.SystemText.ModelExt
{

    /// <summary>
    /// https://w3c.github.io/did-core/
    /// </summary>
    [DebuggerDisplay("DidDocument(Id = {Id})")]
    [DataContract]
    public class DidDocumentExt: IDidDocument
    {
        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        [JsonPropertyName("@context")]
        public IContext? Context { get; set; }

        //TODO: Add alsoKnownAs attribute. How is it modelled in the document? Continue the one GH thread.

        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        [JsonPropertyName("id")]
        public Uri? Id { get; set; }

        //TODO: Make this a Controller class, maybe with implicit and explicit conversion to and from string. Same for some key formats?
        /// <summary>
        /// https://w3c.github.io/did-core/#control
        /// </summary>
        [JsonPropertyName("controller")]
        public string[]? Controller { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#verification-methods
        /// </summary>
        [JsonPropertyName("verificationMethod")]
        public IVerificationMethod[]? VerificationMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#authentication
        /// </summary>
        [JsonPropertyName("authentication")]
        public IAuthenticationMethod[]? Authentication { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#assertionmethod
        /// </summary>
        [JsonPropertyName("assertionMethod")]
        public IAssertionMethod[]? AssertionMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#keyagreement
        /// </summary>
        [JsonPropertyName("keyAgreement")]
        public IKeyAgreementMethod[]? KeyAgreement { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilitydelegation
        /// </summary>
        [JsonPropertyName("capabilityDelegation")]
        public ICapabilityDelegationMethod[]? CapabilityDelegation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilityinvocation
        /// </summary>
        [JsonPropertyName("capabilityInvocation")]
        public ICapabilityInvocationMethod[]? CapabilityInvocation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#service-endpoints
        /// </summary>
        [JsonPropertyName("service")]
        public IService[]? Service { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object>? AdditionalData { get; set; }

    }
}

