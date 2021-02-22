using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using DidNet.Common.Verification;

namespace DidNet.Common
{
    /// <summary>
    /// https://w3c.github.io/did-core/
    /// </summary>
    [DebuggerDisplay("DidDocument(Id = {Id})")]
    [DataContract]
    public class DidDocument : IEquatable<DidDocument>, IDidDocument
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

        //TODO: The following as JSON Extension data + plus inherited from the converter?

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#created
        /// </summary>
        /*[JsonPropertyName("created")]
        public DateTimeOffset? Created { get; set; }
        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#updated
        /// </summary>
        [JsonPropertyName("updated")]
        public DateTimeOffset? Updated { get; set; }*/


        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((DidDocument)obj);
        }


        /// <inheritdoc/>
        public bool Equals(DidDocument? other)
        {
            if (other == null)
            {
                return false;
            }

            return Context == other.Context
                && Id == other?.Id
                && (Controller?.SequenceEqual(other!.Controller!)).GetValueOrDefault()
                && (VerificationMethod?.SequenceEqual(other!.VerificationMethod!)).GetValueOrDefault()
                && (Authentication?.SequenceEqual(other!.Authentication!)).GetValueOrDefault()
                && (AssertionMethod?.SequenceEqual(other!.AssertionMethod!)).GetValueOrDefault()
                && (KeyAgreement?.SequenceEqual(other!.KeyAgreement!)).GetValueOrDefault()
                && (CapabilityDelegation?.SequenceEqual(other!.CapabilityDelegation!)).GetValueOrDefault()
                && (Service?.SequenceEqual(other!.Service!)).GetValueOrDefault();
        }


        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Context);
            hash.Add(Id);

            for (int i = 0; i < Controller?.Length; ++i)
            {
                hash.Add(Controller[i]);
            }

            for (int i = 0; i < VerificationMethod?.Length; ++i)
            {
                hash.Add(VerificationMethod[i]);
            }

            for (int i = 0; i < Authentication?.Length; ++i)
            {
                hash.Add(Authentication[i]);
            }

            for (int i = 0; i < AssertionMethod?.Length; ++i)
            {
                hash.Add(AssertionMethod[i]);
            }

            for (int i = 0; i < KeyAgreement?.Length; ++i)
            {
                hash.Add(KeyAgreement[i]);
            }

            for (int i = 0; i < KeyAgreement?.Length; ++i)
            {
                hash.Add(KeyAgreement[i]);
            }

            for (int i = 0; i < CapabilityDelegation?.Length; ++i)
            {
                hash.Add(CapabilityDelegation[i]);
            }

            for (int i = 0; i < Service?.Length; ++i)
            {
                hash.Add(Service[i]);
            }

            return hash.ToHashCode();
        }
    }
}

