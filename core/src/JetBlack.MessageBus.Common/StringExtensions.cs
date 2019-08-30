using System;
using System.Collections.Generic;

namespace JetBlack.MessageBus.Common
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Splits a text string into an enumeration based on a separator.
        /// </summary>
        /// <param name="text">The string to split.</param>
        /// <param name="separators">The separators which divide the string.</param>
        /// <param name="stringSplitOptions">The options to apply on the split</param>
        /// <returns>An enumeration of strings.</returns>
        public static IEnumerable<string> AsEnumerable(this string text, char[] separators, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(text))
                yield break;

            foreach (var part in text.Split(separators, stringSplitOptions))
                yield return part;
        }

        /// <summary>
        /// Splits a text string into an enumeration based on a separator.
        /// </summary>
        /// <param name="text">The string to split.</param>
        /// <param name="separators">The separators which divide the string.</param>
        /// <param name="stringSplitOptions">The options to apply on the split</param>
        /// <returns>An enumeration of strings.</returns>
        public static IEnumerable<string> AsEnumerable(this string text, string[] separators, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(text))
                yield break;

            foreach (var part in text.Split(separators, stringSplitOptions))
                yield return part;
        }
    }
}