using System.Collections.Generic;

namespace DidNet.Common
{

    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#context
    /// </summary>
    public class ContextObj : IContext
    {

        public virtual ICollection<ContextData>? Contexts { get; set; }

        public virtual IDictionary<string, object>? AdditionalData { get; set; }
    }
}

