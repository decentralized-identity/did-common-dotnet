using System.Collections.Generic;

namespace DidNet.Common
{
    public class ContextData
    {
        public string? Contexe { get; }
        public IDictionary<string, string>? EmbeddedContexe { get; }

        public bool IsEmbeddedContexe { get { return EmbeddedContexe != null; } }

        public ContextData(string context) => Contexe = context;

        public ContextData(IDictionary<string, string> context) => EmbeddedContexe = context;

    }
}
