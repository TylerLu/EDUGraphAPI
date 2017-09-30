/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System.Collections.Generic;

namespace EDUGraphAPI
{
    public static class StringExtensions
    {
        private static readonly HashSet<char> DefaultNonWordCharacters = new HashSet<char> { ',', '.', ':', ';' };

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string CropWholeWords(this string value, int length, HashSet<char> nonWordCharacters = null)
        {
            if (value == null) return "";
            if (length < 0) return value;

            if (nonWordCharacters == null)
                nonWordCharacters = DefaultNonWordCharacters;

            if (length >= value.Length) return value;

            var end = length;
            for (int i = end; i > 0; i--)
            {
                if (value[i].IsWhitespace()) break;

                if (nonWordCharacters.Contains(value[i])
                    && (value.Length == i + 1 || value[i + 1] == ' '))
                    break;
                end--;
            }

            if (end == 0) end = length;
            var result = value.Substring(0, end);
            if (result.Length != value.Length) result += "...";
            return result;
        }

        private static bool IsWhitespace(this char character)
        {
            return character == ' ' || character == 'n' || character == 't';
        }
    }
}