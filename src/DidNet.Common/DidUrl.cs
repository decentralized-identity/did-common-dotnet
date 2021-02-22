using System;
using System.Collections.Generic;

namespace DidNet.Common
{
    public class DidUrl
    {

        public DidUrl()
        {
            Params = new Dictionary<string, string>();
        }

        public string Did { get; set; } = null!;

        public string Url { get; set; } = null!;
        public string Method { get; set; } = null!;

        public string Id { get; set; } = null!;

        public string Path { get; set; } = null!;

        public string Fragment { get; set; } = null!;

        public string Query { get; set; } = null!;

        public Dictionary<string, string> Params { get; set; }
    }
}
