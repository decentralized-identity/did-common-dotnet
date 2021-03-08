using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DidNet.Common.Verification;

namespace DidNet.Json.SystemText
{
    /// <summary>
    /// Abc.
    /// </summary>
    /// <remarks>Refactor a hook here if customer converters for relationships are needed. Is this needed?</remarks>
    public class VerificationRelationshipConverterFactory: JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IVerificationRelationship).IsAssignableFrom(typeToConvert);
        }


        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(VerificationRelationshipConverter<>)
                    .MakeGenericType(new Type[] { typeToConvert }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: null,
                culture: null)!;
        }
    }
}