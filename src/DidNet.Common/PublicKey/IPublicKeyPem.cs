namespace DidNet.Common.PublicKey
{
    public interface IPublicKeyPem : IKeyFormat
    {
        string Key { get; set; }
    }
}