using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DidNet.Json.SystemText.Converters
{
    public class JsonCaseNamingPolicy : JsonNamingPolicy
    {
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        public override string? ConvertName(string name)
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
        {
            if (name.StartsWith("@") && name.Length > 1)
            {
                return "@" + name.Substring(1, 1).ToLower() + name.Substring(2);
            }

            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }
    }
}
