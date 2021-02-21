namespace DidNet.Common.KeyTypes
{
    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#verification-method-types
    /// </summary>
    public static class DidRegisteredKeyTypes
    {
        /// <summary>
        /// https://www.w3.org/TR/did-spec-registries/#jwsverificationkey2020
        /// </summary>
        public const string JwsVerificationKey2020 = "jwsVerificationKey2020";

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
}

