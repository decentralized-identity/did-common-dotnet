using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DidNet.Common.Verification;

namespace DidNet.Common
{
    /// <summary>
    /// https://w3c.github.io/did-core/
    /// </summary>
    [DebuggerDisplay("DidDocument(Id = {Id})")]
    public class DidDocument : IEquatable<DidDocument>
    {
        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        [DataMember(Name ="@context")]
        public Context? Context { get; set; }

        //TODO: Add alsoKnownAs attribute. How is it modelled in the document? Continue the one GH thread.

        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        [DataMember(Name ="id")]
        public Uri? Id { get; set; }

        //TODO: Make this a Controller class, maybe with implicit and explicit conversion to and from string. Same for some key formats?
        /// <summary>
        /// https://w3c.github.io/did-core/#control
        /// </summary>
        [DataMember(Name ="controller")]
        public string[]? Controller { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#verification-methods
        /// </summary>
        [DataMember(Name ="verificationMethod")]
        public VerificationMethod[]? VerificationMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#authentication
        /// </summary>
        [DataMember(Name ="authentication")]
        public AuthenticationMethod[]? Authentication { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#assertionmethod
        /// </summary>
        [DataMember(Name ="assertionMethod")]
        public AssertionMethod[]? AssertionMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#keyagreement
        /// </summary>
        [DataMember(Name ="keyAgreement")]
        public KeyAgreementMethod[]? KeyAgreement { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilitydelegation
        /// </summary>
        [DataMember(Name ="capabilityDelegation")]
        public CapabilityDelegationMethod[]? CapabilityDelegation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilityinvocation
        /// </summary>
        [DataMember(Name ="capabilityInvocation")]
        public CapabilityInvocationMethod[]? CapabilityInvocation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#service-endpoints
        /// </summary>
        [DataMember(Name ="service")]
        public Service[]? Service { get; set; }

        public IDictionary<string, object>? AdditionalData { get; set; }

        //TODO: The following as JSON Extension data + plus inherited from the converter?

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#created
        /// </summary>
        /*[DataMember(Name ="created")]
        public DateTimeOffset? Created { get; set; }
        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#updated
        /// </summary>
        [DataMember(Name ="updated")]
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

