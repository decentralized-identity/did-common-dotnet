using System;

namespace DidNet.Common
{
    public interface IService: IAdditionalData
    {
        Uri? Id { get; set; }
        string? Type { get; set; }
        ServiceEndpointData? ServiceEndpoint { get; set; }
    }
}