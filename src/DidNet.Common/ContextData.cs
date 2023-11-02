using System.Collections.Generic;

namespace DidNet.Common
{
    public class ContextData
    {
        public string? Context { get; }
        public IDictionary<string, string>? EmbeddedContext { get; }

        public bool IsEmbeddedContext { get { return EmbeddedContext != null; } }

        public ContextData(string context) => Context = context;

        public ContextData(IDictionary<string, string> context) => EmbeddedContext = context;

    }
}
