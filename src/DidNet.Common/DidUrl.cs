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

        public string Did { get; set; }

        public string Url { get; set; }
        public string Method { get; set; }

        public string Id { get; set; }

        public string Path { get; set; }

        public string Fragment { get; set; }

        public string Query { get; set; }

        public Dictionary<string, string> Params { get; set; }
    }
}
