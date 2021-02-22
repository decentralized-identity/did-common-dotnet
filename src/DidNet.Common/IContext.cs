using System.Collections.Generic;

namespace DidNet.Common
{
    public interface IContext: IAdditionalData
    {
        ICollection<string>? Contexes { get; set; }
    }
}