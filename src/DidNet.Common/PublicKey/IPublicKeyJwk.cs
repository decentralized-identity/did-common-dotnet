namespace DidNet.Common.PublicKey
{
    public interface IPublicKeyJwk: IKeyFormat
    {
        string? Crv { get; set; }
        string? Kid { get; set; }
        string? Kty { get; set; }
        string? X { get; set; }
        string? Y { get; set; }
    }
}