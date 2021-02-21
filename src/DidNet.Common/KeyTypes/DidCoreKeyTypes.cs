namespace DidNet.Common.KeyTypes
{
    //TODO: These not as nameof-attributes with the same and small strings as in the standard!
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
}

