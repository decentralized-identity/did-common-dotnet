using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DotDecentralized.Core.Did
{
    //TODO: Probably the explanations from https://www.w3.org/TR/did-core/#architecture-overview
    //should be added directly to DidDocument.

    //These at https://www.w3.org/TR/did-core/#did-parameters could be extension methods
    //and something like HasService(), GetService() so the core type remains open to extension
    //and simple. These are extensible, although official DID registries are recommended.

    //ICollection -> the order does not matter
    //IList -> the order matters
    //Consider using IReadOnly versions?

    //Check optionality at https://www.w3.org/TR/did-core/#core-properties-for-a-did-document
    //and decide if type checking should be used (now added ad-hoc) or optionality types
    //or both. Type system checks would allow missing types during runtime, which may
    //preferrable, but maybe it should be clear by way of using appropriate runtime
    //types if they are not flagged by correctness-checking functions or exceptions?

    /// <summary>
    /// https://w3c.github.io/did-core/
    /// </summary>
    [DebuggerDisplay("DidDocument(Id = {Id})")]
    public class DidDocument: IEquatable<DidDocument>
    {
        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        [JsonPropertyName("@context")]
        public Context? Context { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#did-subject
        /// </summary>
        [JsonPropertyName("id")]
        public Uri? Id { get; set; }

        //TODO: Make this a real class.
        /// <summary>
        /// https://w3c.github.io/did-core/#also-known-as.
        /// </summary>
        public string[]? AlsoKnownAs { get; set; }

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
        public VerificationMethod[]? VerificationMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#authentication
        /// </summary>
        [JsonPropertyName("authentication")]
        public AuthenticationMethod[]? Authentication { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#assertionmethod
        /// </summary>
        [JsonPropertyName("assertionMethod")]
        public AssertionMethod[]? AssertionMethod { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#keyagreement
        /// </summary>
        [JsonPropertyName("keyAgreement")]
        public KeyAgreementMethod[]? KeyAgreement { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilitydelegation
        /// </summary>
        [JsonPropertyName("capabilityDelegation")]
        public CapabilityDelegationMethod[]? CapabilityDelegation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#capabilityinvocation
        /// </summary>
        [JsonPropertyName("capabilityInvocation")]
        public CapabilityInvocationMethod[]? CapabilityInvocation { get; set; }

        /// <summary>
        /// https://w3c.github.io/did-core/#service-endpoints
        /// </summary>
        [JsonPropertyName("service")]
        public Service[]? Service { get; set; }

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

        //TODO: The following is technical equality, also logical one should be considered.
        //See at https://w3c.github.io/did-core/#also-known-as.

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((DidDocument)obj);
        }


        /// <inheritdoc/>
        public bool Equals(DidDocument? other)
        {
            if(other is null)
            {
                return false;
            }

            return Context == other.Context
                && Id == other?.Id
                && (AlsoKnownAs?.SequenceEqual(other!.AlsoKnownAs!)).GetValueOrDefault()
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

            for(int i = 0; i < AlsoKnownAs?.Length; ++i)
            {
                hash.Add(AlsoKnownAs[i]);
            }

            for(int i = 0; i < Controller?.Length; ++i)
            {
                hash.Add(Controller[i]);
            }

            for(int i = 0; i < VerificationMethod?.Length; ++i)
            {
                hash.Add(VerificationMethod[i]);
            }

            for(int i = 0; i < Authentication?.Length; ++i)
            {
                hash.Add(Authentication[i]);
            }

            for(int i = 0; i < AssertionMethod?.Length; ++i)
            {
                hash.Add(AssertionMethod[i]);
            }

            for(int i = 0; i < KeyAgreement?.Length; ++i)
            {
                hash.Add(KeyAgreement[i]);
            }

            for(int i = 0; i < KeyAgreement?.Length; ++i)
            {
                hash.Add(KeyAgreement[i]);
            }

            for(int i = 0; i < CapabilityDelegation?.Length; ++i)
            {
                hash.Add(CapabilityDelegation[i]);
            }

            for(int i = 0; i < Service?.Length; ++i)
            {
                hash.Add(Service[i]);
            }

            return hash.ToHashCode();
        }
    }


    /// <summary>
    /// https://www.w3.org/TR/did-core/#service-endpoints
    /// </summary>
    [DebuggerDisplay("Service(Id = {Id})")]
    public class Service
    {
        [JsonPropertyName("id")]
        public Uri? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("serviceEndpoint")]
        public string? ServiceEndPoint { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JsonElement>? AdditionalData { get; set; }
    }


    //TODO: These not as nameof-attributes since in the specification they start with
    //small letter while capital letter is a .NET convention.
    /// <summary>
    /// https://w3c.github.io/did-core/#key-types-and-formats
    /// </summary>
    public static class DidCoreKeyTypes
    {
        public const string RsaVerificationKey2018 = "rsaVerificationKey2018";
        public const string Ed25519VerificationKey2018 = "ed25519VerificationKey2018";
        public const string SchnorrSecp256k1VerificationKey2019 = "schnorrSecp256k1VerificationKey2019";
        public const string X25519KeyAgreementKey2019 = "x25519KeyAgreementKey2019";
    }


    /// <summary>
    /// Constants for various cryptographic algorithms used in
    /// decentralized identifiers and verifiable credentials.
    /// </summary>
    public static class CryptographyAlgorithmConstants
    {
        /// <summary>
        /// EdDSA constants.
        /// </summary>
        public static class EdDsa
        {
            public const string Algorithm = "EdDSA";

            /// <summary>
            /// By definition, see at <see href="https://tools.ietf.org/html/rfc8037#section-2"/>.
            /// </summary>
            public const string KeyType = "OKP";

            /// <summary>
            /// By definition, see at <see href="https://tools.ietf.org/html/rfc8032#section-5.1.5"/>.
            /// </summary>
            public const int KeySizeInBytes = 32;

            /// <summary>
            /// EdDSA key curves.
            /// </summary>
            public static class Curves
            {
                //TODO: Add links to definitions as linked in https://tools.ietf.org/html/rfc8037#page-7.
                public const string Ed25519 = "Ed25519";
                public const string Ed448 = "Ed448";
                public const string X25519 = "X25519";
                public const string X448 = "X448";
            }
        }
    }


    //TODO: These not as nameof-attributes since in the specification they start with
    //small letter while capital letter is a .NET convention.
    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#verification-method-types
    /// </summary>
    public static class DidRegisteredKeyTypes
    {
        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#jwsverificationkey2020
        /// </summary>
        public const string JwsVerificationKey2020 ="jwsVerificationKey2020";

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#ecdsasecp256k1verificationkey2019
        /// </summary>
        public const string EcdsaSecp256k1VerificationKey2019 = "ecdsaSecp256k1VerificationKey2019";

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#ed25519verificationkey2018
        /// </summary>
        public const string Ed25519VerificationKey2018 = "ed25519VerificationKey2018";

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#gpgverificationkey2020
        /// </summary>
        public const string GpgVerificationKey2020 = "gpgVerificationKey2020";

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#rsaverificationkey2018
        /// </summary>
        public const string RsaVerificationKey2018 = "rsaVerificationKey2018";

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#x25519keyagreementkey2019
        /// </summary>
        public const string X25519KeyAgreementKey2019 = "x25519KeyAgreementKey2019";

        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#ecdsasecp256k1recoverymethod2020
        /// </summary>
        public const string EcdsaSecp256k1RecoveryMethod2020 = "ecdsaSecp256k1RecoveryMethod2020";
    }

    /// <summary>
    /// https://www.w3.org/TR/did-core/#key-types-and-formats
    /// </summary>
    public abstract class KeyFormat { }


    [DebuggerDisplay("PublicKeyHex({Key})")]
    public class PublicKeyHex: KeyFormat
    {
        public string Key { get; set; }

        public PublicKeyHex(string key)
        {
            Key = key ?? throw new ArgumentException(nameof(key));
        }
    }


    [DebuggerDisplay("PublicKeyBase58({Key})")]
    public class PublicKeyBase58: KeyFormat
    {
        public string Key { get; set; }

        public PublicKeyBase58(string key)
        {
            Key = key ?? throw new ArgumentException(nameof(key));
        }
    }


    [DebuggerDisplay("PublicKeyPem({Key})")]
    public class PublicKeyPem: KeyFormat
    {
        public string Key { get; set; }

        public PublicKeyPem(string key)
        {
            Key = key ?? throw new ArgumentException(nameof(key));
        }
    }

    /// <summary>
    /// https://www.w3.org/TR/did-core/#dfn-publickeyjwk
    /// </summary>
    /// <remarks>Note that must not contain private key information, such as 'd' field,
    /// by DID Core specification.</remarks>
    [DebuggerDisplay("PublicKeyJwk(Crv = {Crv}, Kid = {Kid}, Kty = {Kty}, X = {X}, Y = {Y})")]
    public class PublicKeyJwk: KeyFormat
    {
        [JsonPropertyName("crv")]
        public string? Crv { get; set; }

        [JsonPropertyName("kid")]
        public string? Kid { get; set; }

        [JsonPropertyName("kty")]
        public string? Kty { get; set; }

        [JsonPropertyName("x")]
        public string? X { get; set; }

        [JsonPropertyName("y")]
        public string? Y { get; set; }
    }



    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("VerificationMethod(Id = {Id})")]
    public class VerificationMethod
    {
        //TODO: Could be FractionOrUri: Uri, or C# 10/F# discriminated union (like VerificationRelationship would be).
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("controller")]
        public string? Controller { get; set; }

        public KeyFormat? KeyFormat { get; set; }
    }


    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// The reference Id field is string because it can be a fragment like "#key-1".
    /// </summary>
    [DebuggerDisplay("VerificationRelationship(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public abstract class VerificationRelationship
    {
        public string? VerificationReferenceId { get; }
        public VerificationMethod? EmbeddedVerification { get; }

        protected VerificationRelationship(string verificationReferenceId) => VerificationReferenceId = verificationReferenceId;
        protected VerificationRelationship(VerificationMethod embeddedVerification) => EmbeddedVerification = embeddedVerification;

        public string? Id => EmbeddedVerification == null ? VerificationReferenceId : EmbeddedVerification.Id?.ToString();

        public bool IsEmbeddedVerification { get { return EmbeddedVerification != null; } }
    }


    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("AuthenticationMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class AuthenticationMethod: VerificationRelationship
    {
        public AuthenticationMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public AuthenticationMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }

    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("AssertionMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class AssertionMethod: VerificationRelationship
    {
        public AssertionMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public AssertionMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }


    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("KeyAgreementMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class KeyAgreementMethod: VerificationRelationship
    {
        public KeyAgreementMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public KeyAgreementMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }

    /// <summary>
    /// https://www.w3.org/TR/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("CapabilityDelegationMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class CapabilityDelegationMethod: VerificationRelationship
    {
        public CapabilityDelegationMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public CapabilityDelegationMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }

    /// <summary>
    /// https://w3c.github.io/did-core/#verification-methods
    /// </summary>
    [DebuggerDisplay("CapabilityInvocationMethod(Id = {Id}, IsEmbeddedVerification = {IsEmbeddedVerification})")]
    public class CapabilityInvocationMethod: VerificationRelationship
    {
        public CapabilityInvocationMethod(string verificationReferenceId) : base(verificationReferenceId) { }
        public CapabilityInvocationMethod(VerificationMethod embeddedVerification) : base(embeddedVerification) { }
    }


    //TODO: The extension data here is a quick'n'dirty way to allow for anything, e.g.
    //nested complex types in case such are encountered.
    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#context
    /// </summary>
    public class Context
    {
        public ICollection<string>? Contexes { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object>? AdditionalData { get; set; }
    }
}
