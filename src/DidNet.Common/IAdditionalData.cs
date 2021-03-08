using System.Collections.Generic;

namespace DidNet.Common
{
    public interface IAdditionalData
    {
        IDictionary<string, object>? AdditionalData { get; set; }
    }
}