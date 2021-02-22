using System;
using System.Diagnostics;

namespace DidNet.Common.PublicKey
{
    [DebuggerDisplay("PublicKeyBase58({Key})")]
    public class PublicKeyBase58 : KeyFormat, IPublicKeyBase58
    {
        public string Key { get; set; }

        public PublicKeyBase58(string key)
        {
            Key = key ?? throw new ArgumentException(nameof(key));
        }
    }
}

