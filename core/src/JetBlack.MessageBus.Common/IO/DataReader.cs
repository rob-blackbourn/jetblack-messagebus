#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JetBlack.MessageBus.Common.IO
{
    /// <summary>
    /// A data reader.
    /// </summary>
    public class DataReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveInnerStreamOpen;

        /// <summary>
        /// Construct a data reader.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="leaveInnerStreamOpen">If true leave the inner stream open when disposing.</param>
        public DataReader(Stream stream, bool leaveInnerStreamOpen = true)
        {
            _stream = stream;
            _leaveInnerStreamOpen = leaveInnerStreamOpen;
        }

        /// <summary>
        /// Read a boolean.
        /// </summary>
        /// <returns>The boolean read.</returns>
        public bool ReadBoolean()
        {
            return (_stream.ReadByte() != 0);
        }

        /// <summary>
        /// Read a byte.
        /// </summary>
        /// <returns>The byte read</returns>
        public byte ReadByte()
        {
            var ch = _stream.ReadByte();
            if (ch < 0)
                throw new EndOfStreamException();
            return (byte)ch;
        }

        /// <summary>
        /// Read an int.
        /// </summary>
        /// <returns>The int read</returns>
        public int ReadInt32()
        {
            var buf = ReadFully(new byte[4]);
            return
                (buf[0] << 24) +
                (buf[1] << 16) +
                (buf[2] << 8) +
                (buf[3] << 0);
        }

        /// <summary>
        /// Read a string.
        /// </summary>
        /// <returns>The string read</returns>
        public string ReadString()
        {
            var len = ReadInt32();
            var buf = ReadFully(new byte[len]);
            return Encoding.UTF8.GetString(buf);
        }

        public ISet<int>? ReadInt32Set()
        {
            var count = ReadInt32();
            if (count == 0)
                return null;
            var set = new HashSet<int>();
            for (var i = 0; i < count; ++i)
                set.Add(ReadInt32());
            return set;
        }

        /// <summary>
        /// Read an array of bytes.
        /// </summary>
        /// <returns>The array of bytes read</returns>
        public byte[]? ReadByteArray()
        {
            var count = ReadInt32();
            if (count == 0)
                return null;
            else
            {
                var data = new byte[count];
                var buf = ReadFully(data);
                return buf;
            }
        }

        /// <summary>
        /// Read a guid.
        /// </summary>
        /// <returns>The guid read</returns>
        public Guid ReadGuid()
        {
            var buf = new byte[16];
            ReadFully(buf);
            return new Guid(buf);
        }

        public DataPacket ReadBinaryDataPacket()
        {
            var entitlements = ReadInt32Set();
            var data = ReadByteArray();
            return new DataPacket(entitlements, data);
        }

        public DataPacket[]? ReadBinaryDataPacketArray()
        {
            var count = ReadInt32();
            if (count == 0)
                return null;

            DataPacket[] dataPackets = new DataPacket[count];
            for (var i = 0; i < count; ++i)
                dataPackets[i] = ReadBinaryDataPacket();
            return dataPackets;
        }

        private byte[] ReadFully(byte[] buf)
        {
            return ReadFully(buf, 0, buf.Length);
        }

        private byte[] ReadFully(byte[] buf, int off, int len)
        {
            if (len < 0)
                throw new IndexOutOfRangeException();

            var n = 0;
            while (n < len)
            {
                var count = _stream.Read(buf, off + n, len - n);
                if (count < 0)
                    throw new EndOfStreamException();
                n += count;
            }

            return buf;
        }

        public void Dispose()
        {
            if (!_leaveInnerStreamOpen)
                _stream.Dispose();
        }
    }
}