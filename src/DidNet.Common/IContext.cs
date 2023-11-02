using System.Collections.Generic;

namespace DidNet.Common
{
    public interface IContext: IAdditionalData
    {
        ICollection<ContextData>? Contexts { get; set; }
    }
}