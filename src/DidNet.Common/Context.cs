using System.Collections.Generic;

namespace DidNet.Common
{
    /// <summary>
    /// https://www.w3.org/TR/did-spec-registries/#context
    /// </summary>
    public class Context
    {
        public ICollection<string>? Contexes { get; set; }

        public IDictionary<string, object>? AdditionalData { get; set; }
    }
}

