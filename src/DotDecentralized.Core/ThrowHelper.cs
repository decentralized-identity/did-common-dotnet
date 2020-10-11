using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DotDecentralized.Core
{
    /// <summary>
    /// Helper methods to throw. Extracting throw makes conditional branches smaller, more inlinable and reduces
    /// generic codegen size.
    /// </summary>
    /// <remarks>Based on https://github.com/dotnet/corefx/blob/e1991a4f9c5cff43908a03d2c787e8246cfa5583/src/System.Text.Json/src/System/Text/Json/ThrowHelper.cs
    /// and https://github.com/dotnet/corefx/blob/master/src/System.Text.Json/src/System/Text/Json/ThrowHelper.Serialization.cs .</remarks>
    public static class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowJsonException()
        {
            throw new JsonException();
        }
    }
}