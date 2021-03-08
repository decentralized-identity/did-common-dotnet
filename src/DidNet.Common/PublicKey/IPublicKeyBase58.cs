namespace DidNet.Common.PublicKey
{
    public interface IPublicKeyBase58: IKeyFormat
    {
        string Key { get; set; }
    }
}