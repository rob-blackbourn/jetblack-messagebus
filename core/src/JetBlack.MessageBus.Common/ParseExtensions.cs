#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace JetBlack.MessageBus.Common
{
    /// <summary>
    /// A parsing delegate for the try-parse pattern.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <param name="result">The variable to receive the parsed value.</param>
    /// <typeparam name="TResult">The type of the value to parse.</typeparam>
    /// <returns>If the parse was successful then <c>true</c>, otherwise <c>false</c>.</returns>
    public delegate bool TryParseDelegate<TResult>(string s, out TResult result);

    /// <summary>
    /// Extension methods for parsing
    /// </summary>
    public static class ParseExtensions
    {
        /// <summary>
        /// Tries to parse the given text as an int, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>Either an integer or null.</returns>
        public static int? AsInt(this string text)
        {
            int value;
            return int.TryParse(text, out value) ? value : (int?)null;
        }

        /// <summary>
        /// Tries to parse the given text as an int, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="style">The number styles to obey.</param>
        /// <param name="provider">The provider to use</param>
        /// <returns>Either an integer or null</returns>
        public static int? AsInt(this string text, NumberStyles style, IFormatProvider provider)
        {
            int value;
            return int.TryParse(text, style, provider, out value) ? value : (int?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a short, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>Either a short or null</returns>
        public static short? AsShort(this string text)
        {
            short value;
            return short.TryParse(text, out value) ? value : (short?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a short, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="style">The number styles to obey.</param>
        /// <param name="provider">The provider to use</param>
        /// <returns>Either a short or null</returns>
        public static short? AsShort(this string text, NumberStyles style, IFormatProvider provider)
        {
            short value;
            return short.TryParse(text, style, provider, out value) ? value : (short?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a long, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>Either a long or null</returns>
        public static long? AsLong(this string text)
        {
            long value;
            return long.TryParse(text, out value) ? value : (long?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a long, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="style">The number styles to obey.</param>
        /// <param name="provider">The provider to use</param>
        /// <returns>Either a long or null</returns>
        public static long? AsLong(this string text, NumberStyles style, IFormatProvider provider)
        {
            long value;
            return long.TryParse(text, style, provider, out value) ? value : (long?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a float, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>Either a float or null</returns>
        public static float? AsFloat(this string text)
        {
            float value;
            return float.TryParse(text, out value) ? value : (float?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a float, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="style">The number styles to obey.</param>
        /// <param name="provider">The provider to use</param>
        /// <returns>Either a float or null</returns>
        public static float? AsFloat(this string text, NumberStyles style, IFormatProvider provider)
        {
            float value;
            return float.TryParse(text, style, provider, out value) ? value : (float?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a double, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>Either a double or null</returns>
        public static double? AsDouble(this string text)
        {
            double value;
            return double.TryParse(text, out value) ? value : (double?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a double, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="style">The number styles to obey.</param>
        /// <param name="provider">The provider to use</param>
        /// <returns>Either a double or null</returns>
        public static double? AsDouble(this string text, NumberStyles style, IFormatProvider provider)
        {
            double value;
            return double.TryParse(text, style, provider, out value) ? value : (double?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a decimal, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>Either a decimal or null</returns>
        public static decimal? AsDecimal(this string text)
        {
            decimal value;
            return decimal.TryParse(text, out value) ? value : (decimal?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a decimal, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="style">The number styles to obey.</param>
        /// <param name="provider">The provider to use</param>
        /// <returns>Either a decimal or null</returns>
        public static decimal? AsDecimal(this string text, NumberStyles style, IFormatProvider provider)
        {
            decimal value;
            return decimal.TryParse(text, style, provider, out value) ? value : (decimal?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a date time, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <returns>Either a date time or null</returns>
        public static DateTime? AsDateTime(this string text)
        {
            DateTime value;
            return DateTime.TryParse(text, out value) ? value : (DateTime?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a date time, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="style">The styles to obey.</param>
        /// <param name="provider">The provider to use</param>
        /// <returns>Either a date time or null</returns>
        public static DateTime? AsDateTime(this string text, IFormatProvider provider, DateTimeStyles style)
        {
            DateTime value;
            return DateTime.TryParse(text, provider, style, out value) ? value : (DateTime?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a date time, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="format">The format to use</param>
        /// <param name="provider">The provider to use</param>
        /// <param name="style">The styles to obey.</param>
        /// <returns>Either a date time or null</returns>
        public static DateTime? AsDateTime(
            this string text,
            string format,
            IFormatProvider? provider = null,
            DateTimeStyles style = DateTimeStyles.None)
        {
            DateTime value;
            return DateTime.TryParseExact(text, format, provider, style, out value) ? value : (DateTime?)null;
        }

        /// <summary>
        /// Tries to parse the given text as a date time, returning null on failure.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="formats">The formats to use</param>
        /// <param name="provider">The provider to use</param>
        /// <param name="style">The styles to obey.</param>
        /// <returns>Either a date time or null</returns>
        public static DateTime? AsDateTime(
            this string text,
            string[] formats,
            IFormatProvider? provider = null,
            DateTimeStyles style = DateTimeStyles.None)
        {
            DateTime value;
            return DateTime.TryParseExact(text, formats, provider, style, out value) ? value : (DateTime?)null;
        }

        /// <summary>
        /// Converts a string to a nullable time span.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>If the text could be parse a nullable time span, otherwise null.</returns>
        public static TimeSpan? AsTimeSpan(this string text)
        {
            TimeSpan value;
            if (!string.IsNullOrWhiteSpace(text) && TimeSpan.TryParse(text, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Converts a string to a nullable time span.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="format">The format string</param>
        /// <param name="provider">A format provider</param>
        /// <returns>If the text could be parse a nullable time span, otherwise null.</returns>
        public static TimeSpan? AsTimeSpan(this string text, string format, IFormatProvider provider)
        {
            TimeSpan value;
            if (!string.IsNullOrWhiteSpace(text) && TimeSpan.TryParseExact(text, format, provider, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Converts a string to a nullable time span.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="format">The format string</param>
        /// <param name="provider">A format provider</param>
        /// <param name="style">The styles to obey</param>
        /// <returns>If the text could be parse a nullable time span, otherwise null.</returns>
        public static TimeSpan? AsTimeSpan(this string text, string format, IFormatProvider provider, TimeSpanStyles style)
        {
            TimeSpan value;
            if (!string.IsNullOrWhiteSpace(text) && TimeSpan.TryParseExact(text, format, provider, style, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Converts a string to a nullable time span.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="formats">The formats to use</param>
        /// <param name="provider">A format provider</param>
        /// <returns>If the text could be parse a nullable time span, otherwise null.</returns>
        public static TimeSpan? AsTimeSpan(this string text, string[] formats, IFormatProvider provider)
        {
            TimeSpan value;
            if (!string.IsNullOrWhiteSpace(text) && TimeSpan.TryParseExact(text, formats, provider, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Converts a string to a nullable time span.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="formats">The formats to use</param>
        /// <param name="provider">A format provider</param>
        /// <param name="style">The styles to obey</param>
        /// <returns>If the text could be parse a nullable time span, otherwise null.</returns>
        public static TimeSpan? AsTimeSpan(this string text, string[] formats, IFormatProvider provider, TimeSpanStyles style)
        {
            TimeSpan value;
            if (!string.IsNullOrWhiteSpace(text) && TimeSpan.TryParseExact(text, formats, provider, style, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Converts a string to a nullable time span.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="provider">A format provider</param>
        /// <returns>If the text could be parse a nullable time span, otherwise null.</returns>
        public static TimeSpan? AsTimeSpan(this string text, IFormatProvider provider)
        {
            TimeSpan value;
            if (!string.IsNullOrWhiteSpace(text) && TimeSpan.TryParse(text, provider, out value))
                return value;
            return null;
        }

        /// <summary>
        /// Creates an enumeration of time spans based on a separator delimited string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static IEnumerable<TimeSpan> AsTimeSpanEnumerable(this string text, params char[] separators)
        {
#nullable disable
            return text
                .AsEnumerable(AsTimeSpan, separators, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.HasValue)
                .Select(x => x.Value);
#nullable enable
        }

        private static IEnumerable<T> AsEnumerable<T>(this string text, Func<string, T> projector, char[] separators, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return text.AsEnumerable(separators, stringSplitOptions).Select(projector);
        }

        public static IPAddress? AsIPAddress(this string text)
        {
            IPAddress address;
            if (!IPAddress.TryParse(text, out address))
                return null;
            return address;
        }

        public static IEnumerable<IPAddress> AsIPAddressEnumerable(this string text, params char[] separators)
        {
#nullable disable
            return text.AsEnumerable(AsIPAddress, separators, StringSplitOptions.RemoveEmptyEntries).Where(x => x != null);
#nullable enable
        }
    }
}