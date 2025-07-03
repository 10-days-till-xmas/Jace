using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jace.Util
{
    /// <summary>
    /// Utility methods of Jace.NET that can be used throughout the engine.
    /// </summary>
    internal  static class EngineUtil
    {
        // TODO: Use Dictionary<string, T>(StringComparer.OrdinalIgnoreCase) instead of these method
        internal static IDictionary<string, double> ConvertVariableNamesToLowerCase(IDictionary<string, double> variables)
        {
            return variables.ToDictionary(keyValuePair => keyValuePair.Key.ToLowerFast(), keyValuePair => keyValuePair.Value);
        }

        // This is a fast ToLower for strings that are in ASCII
        internal static string ToLowerFast(this string text)
        {
            var buffer = new StringBuilder(text.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for(var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (c is >= 'A' and <= 'Z') // 'A' <= c <= 'Z'
                {
                    buffer.Append((char)(c + 32));
                }
                else 
                {
                    buffer.Append(c);
                }
            }

            return buffer.ToString();
        }
    }
}
