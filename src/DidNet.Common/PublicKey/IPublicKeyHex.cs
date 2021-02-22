namespace DidNet.Common.PublicKey
{
    public interface IPublicKeyHex: IKeyFormat
    {
        string Key { get; set; }
    }
}