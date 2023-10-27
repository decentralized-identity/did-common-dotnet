using DidDocumentResolver.DifCommonModels.Common;
using System.Collections.Generic;

namespace DidNet.Common
{
    public interface IContext: IAdditionalData
    {
        ICollection<ContextData>? Contexes { get; set; }
    }
}