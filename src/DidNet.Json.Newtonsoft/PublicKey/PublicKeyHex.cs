using System;
using System.Diagnostics;
using DidNet.Common.PublicKey;

namespace DidNet.Json.Newtonsoft.PublicKey
{
    [DebuggerDisplay("PublicKeyHex({Key})")]
    public class PublicKeyHex : KeyFormat, IPublicKeyHex
    {
        public string Key { get; set; }

        public PublicKeyHex(string key)
        {
            Key = key ?? throw new ArgumentException(nameof(key));
        }
    }
}

