using System.Collections.Generic;
using Newtonsoft.Json;

namespace DidNet.Json.Newtonsoft.Converters.Builders
{
    public interface IJsonConverterBuilder
    {
        IEnumerable<JsonConverter> GetConverters();
    }
}