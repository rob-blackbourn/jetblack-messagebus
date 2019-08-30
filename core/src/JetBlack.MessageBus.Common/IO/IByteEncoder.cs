#nullable enable

namespace JetBlack.MessageBus.Common.IO
{
    /// <summary>
    /// An interface for a class which can encode and decode byte arrays.
    /// </summary>
    public interface IByteEncoder
    {
        /// <summary>
        /// Decode an object from an array of bytes.
        /// </summary>
        /// <param name="bytes">The array to decode.</param>
        /// <returns>The object represented by the byte array.</returns>
        object? Decode(byte[]? bytes);

        /// <summary>
        /// Encode an object into an array of bytes.
        /// </summary>
        /// <param name="data">The object to encode.</param>
        /// <returns>The object encoded as an array of bytes.</returns>
        byte[]? Encode(object? data);
    }
}
