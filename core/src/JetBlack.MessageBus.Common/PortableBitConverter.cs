#nullable enable

using System;

namespace JetBlack.MessageBus.Common
{
    /// <summary>
    /// This class is an adjunct to System.BitConverter. It's purpose is to provide
    /// platform independent methods to transform types into byte arrays and back.
    /// </summary>
    public static class PortableBitConverter
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// Convert a signed long to a date time.
        /// </summary>
        /// <param name="timestamp">The timestamp as a long in milliseconds since 1970-01-01.</param>
        /// <returns>The date time corresponding to the milliseconds since 1970-01-01</returns>
        public static DateTime Int64ToDate(this long timestamp)
        {
            return Epoch.AddMilliseconds(timestamp);
        }

        /// <summary>
        /// Converts a date time to a long as the number of milliseconds since 1970-01-01.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>A long value which is the number of milliseconds since 1970-01-01.</returns>
        public static long DateToInt64(this DateTime date)
        {
            var diff = date - Epoch;
            return (long)Math.Floor(diff.TotalMilliseconds);
        }

        /// <summary>
        /// Converts a byte array to a date time.
        /// </summary>
        /// <param name="buf">The byte array containing the date time</param>
        /// <param name="offset">An offset into the byte stream</param>
        /// <returns>The decoded date time value.</returns>
        public static DateTime ToDateTime(this byte[] buf, int offset = 0)
        {
            var timestamp = ToInt64(buf, offset);
            return Int64ToDate(timestamp);
        }

        /// <summary>
        /// Converts a date time into a byte stream.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A stream of bytes representing the date time.</returns>
        public static byte[] GetBytes(this DateTime value)
        {
            var timestamp = DateToInt64(value);
            return GetBytes(timestamp);
        }

        /// <summary>
        /// Converts a byte array into a short.
        /// </summary>
        /// <param name="buf">The byte array.</param>
        /// <param name="offset">A start offset into the byte array.</param>
        /// <returns>A decoded short value.</returns>
        public static short ToInt16(this byte[] buf, int offset = 0)
        {
            return (short)((buf[offset] << 8) + (buf[offset + 1] << 0));
        }

        /// <summary>
        /// Converts a byte array into a signed int.
        /// </summary>
        /// <param name="buf">The byte array.</param>
        /// <param name="offset">A start offset into the byte array.</param>
        /// <returns>A decoded signed int value.</returns>
        public static int ToInt32(this byte[] buf, int offset = 0)
        {
            return
                (buf[offset] << 24) +
                (buf[offset + 1] << 16) +
                (buf[offset + 2] << 8) +
                (buf[offset + 3] << 0);
        }

        /// <summary>
        /// Converts a byte array into a signed long.
        /// </summary>
        /// <param name="buf">The byte array.</param>
        /// <param name="offset">A start offset into the byte array.</param>
        /// <returns>A decoded signed long value.</returns>
        public static long ToInt64(this byte[] buf, int offset = 0)
        {
            return (((long)buf[offset] << 56) +
                    ((long)(buf[offset + 1] & 255) << 48) +
                    ((long)(buf[offset + 2] & 255) << 40) +
                    ((long)(buf[offset + 3] & 255) << 32) +
                    ((long)(buf[offset + 4] & 255) << 24) +
                    ((buf[offset + 5] & 255) << 16) +
                    ((buf[offset + 6] & 255) << 8) +
                    ((buf[offset + 7] & 255) << 0));
        }

        /// <summary>
        /// Converts a byte array into a float.
        /// </summary>
        /// <param name="buf">The byte array.</param>
        /// <param name="offset">A start offset into the byte array.</param>
        /// <returns>A decoded float value.</returns>
        public static float ToFloat(this byte[] buf, int offset = 0)
        {
            byte[] byteArray = { buf[offset + 3], buf[offset + 2], buf[offset + 1], buf[offset] };
            return BitConverter.ToSingle(byteArray, offset);
        }

        /// <summary>
        /// Converts a byte array into a double.
        /// </summary>
        /// <param name="buf">The byte array.</param>
        /// <param name="offset">A start offset into the byte array.</param>
        /// <returns>A decoded double value.</returns>
        public static double ToDouble(this byte[] buf, int offset = 0)
        {
            var value = ToInt64(buf, offset);
            return BitConverter.Int64BitsToDouble(value);
        }

        /// <summary>
        /// Converts a byte array into a char.
        /// </summary>
        /// <param name="buf">The byte array.</param>
        /// <param name="offset">A start offset into the byte array.</param>
        /// <returns>A decoded char value.</returns>
        public static char ToChar(this byte[] buf, int offset = 0)
        {
            return (char)((buf[offset] << 8) + (buf[offset + 1] << 0));
        }

        /// <summary>
        /// Converts a char into a byte stream.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A stream of bytes representing the char.</returns>
        public static byte[] GetBytes(this char value)
        {
            return new[]
            {
                (byte) ((value >> 8) & 0xFF),
                (byte) ((value >> 0) & 0xFF)
            };
        }

        /// <summary>
        /// Converts a signed int into a byte stream.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A stream of bytes representing the signed int.</returns>
        public static byte[] GetBytes(this int value)
        {
            return new[]
            {
                (byte) ((value >> 24) & 0xFF),
                (byte) ((value >> 16) & 0xFF),
                (byte) ((value >> 8) & 0xFF),
                (byte) ((value >> 0) & 0xFF)
            };
        }

        /// <summary>
        /// Converts a signed long into a byte stream.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A stream of bytes representing the signed long.</returns>
        public static byte[] GetBytes(this long value)
        {
            return new[]
            {
                (byte) ((value >> 56) & 0xFF),
                (byte) ((value >> 48) & 0xFF),
                (byte) ((value >> 40) & 0xFF),
                (byte) ((value >> 32) & 0xFF),
                (byte) ((value >> 24) & 0xFF),
                (byte) ((value >> 16) & 0xFF),
                (byte) ((value >> 8) & 0xFF),
                (byte) ((value >> 0) & 0xFF)
            };
        }

        /// <summary>
        /// Converts a signed short into a byte stream.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A stream of bytes representing the signed short.</returns>
        public static byte[] GetBytes(this short value)
        {
            return new[]
            {
                (byte) ((value >> 8) & 0xFF),
                (byte) ((value >> 0) & 0xFF)
            };
        }

        /// <summary>
        /// Converts a float into a byte stream.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A stream of bytes representing the float.</returns>
        public static byte[] GetBytes(this float value)
        {
            var byteArray = BitConverter.GetBytes(value);
            Array.Reverse(byteArray);
            return byteArray;
        }

        /// <summary>
        /// Converts a double into a byte stream.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>A stream of bytes representing the double.</returns>
        public static byte[] GetBytes(this double value)
        {
            var byteArray = BitConverter.GetBytes(value);
            Array.Reverse(byteArray);
            return byteArray;
        }
    }
}
