using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DidNet.Common
{

    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#context
    /// </summary>
    public class Context : IContext
    {
        public virtual ICollection<string>? Contexes { get; set; }

        public virtual IDictionary<string, object>? AdditionalData { get; set; }
    }
}

