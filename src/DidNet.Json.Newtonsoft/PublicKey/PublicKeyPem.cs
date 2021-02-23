using System;
using System.Diagnostics;
using DidNet.Common.PublicKey;

namespace DidNet.Json.Newtonsoft.PublicKey
{
    [DebuggerDisplay("PublicKeyPem({Key})")]
    public class PublicKeyPem : KeyFormat, IPublicKeyPem
    {
        public string Key { get; set; }

        public PublicKeyPem(string key)
        {
            Key = key ?? throw new ArgumentException(nameof(key));
        }
    }
}

