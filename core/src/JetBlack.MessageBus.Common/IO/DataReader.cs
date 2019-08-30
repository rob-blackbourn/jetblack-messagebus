#nullable enable

using System;
using System.IO;
using System.Net;
using System.Text;

namespace JetBlack.MessageBus.Common.IO
{
    public class DataReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveInnerStreamOpen;

        public DataReader(Stream stream, bool leaveInnerStreamOpen = true)
        {
            _stream = stream;
            _leaveInnerStreamOpen = leaveInnerStreamOpen;
        }

        /// <summary>
        /// Read an array of bytes.
        /// </summary>
        /// <param name="value">The buffer to hold the data</param>
        /// <returns>The count of bytes read</returns>
        public int Read(byte[] value)
        {
            return _stream.Read(value, 0, value.Length);
        }

        /// <summary>
        /// Read an array of bytes from a stream into an array at a given offset.
        /// </summary>
        /// <param name="value">The buffer to hold the data</param>
        /// <param name="off">The offset into the array where the data will be written</param>
        /// <param name="len">The number of bytes to read</param>
        /// <returns>The count of bytes read</returns>
        public int Read(byte[] value, int off, int len)
        {
            return _stream.Read(value, off, len);
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
        /// Read a character.
        /// </summary>
        /// <returns>The character read</returns>
        public char ReadChar()
        {
            return ReadFully(new byte[2]).ToChar();
        }

        /// <summary>
        /// Read a double.
        /// </summary>
        /// <returns>The double read</returns>
        public double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(ReadInt64());
        }

        /// <summary>
        /// Read a float.
        /// </summary>
        /// <returns>The float read</returns>
        public float ReadFloat()
        {
            return ReadFully(new byte[4]).ToFloat();
        }

        /// <summary>
        /// Read a short.
        /// </summary>
        /// <returns>The short read</returns>
        public short ReadInt16()
        {
            return ReadFully(new byte[2]).ToInt16();
        }

        /// <summary>
        /// Read an unsigned short.
        /// </summary>
        /// <returns>The unsigned short read</returns>
        public ushort ReadUInt16()
        {
            return (ushort)ReadFully(new byte[2]).ToInt16();
        }

        /// <summary>
        /// Read an int.
        /// </summary>
        /// <returns>The int read</returns>
        public int ReadInt32()
        {
            return ReadFully(new byte[4]).ToInt32();
        }

        /// <summary>
        /// Read a long.
        /// </summary>
        /// <returns>The long read</returns>
        public long ReadInt64()
        {
            return ReadFully(new byte[8]).ToInt64();
        }

        /// <summary>
        /// Read an ip address.
        /// </summary>
        /// <returns>The ip address read</returns>
        public IPAddress ReadIPAddress()
        {
            var len = ReadInt32();
            var address = new byte[len];
            ReadFully(address);
            return new IPAddress(address);
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

        /// <summary>
        /// Read an array of strings.
        /// </summary>
        /// <returns>The array of strings read</returns>
        public string[]? ReadArrayOfStrings()
        {
            var count = ReadInt32();
            if (count == 0)
                return null;
            var array = new string[count];
            for (var i = 0; i < count; ++i)
                array[i] = ReadString();
            return array;
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
        /// Read an array of byte arrays.
        /// </summary>
        /// <returns>The array of byte arrays read</returns>
        public byte[]?[]? ReadArrayOfByteArrays()
        {
            var count = ReadInt32();
            if (count == 0)
                return null;
            byte[]?[] array = new byte[count][];
            for (var i = 0; i < count; ++i)
                array[i] = ReadByteArray();
            return array;
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

        /// <summary>
        /// Read an array of guids.
        /// </summary>
        /// <returns>The array of guids read</returns>
        public Guid[]? ReadGuidArray()
        {
            var count = ReadInt32();
            if (count == 0)
                return null;

            var array = new Guid[count];
            for (var i = 0; i < count; ++i)
                array[i] = ReadGuid();

            return array;
        }

        public BinaryDataPacket ReadBinaryDataPacket()
        {
            var header = ReadGuid();
            var body = ReadByteArray();
            return new BinaryDataPacket(header, body);
        }

        public BinaryDataPacket[]? ReadBinaryDataPacketArray()
        {
            var count = ReadInt32();
            if (count == 0)
                return null;

            BinaryDataPacket[] data = new BinaryDataPacket[count];
            for (var i = 0; i < count; ++i)
                data[i] = ReadBinaryDataPacket();
            return data;
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

        /// <summary>
        /// Read a date.
        /// </summary>
        /// <returns>The date read</returns>
        public DateTime ReadDate()
        {
            return ReadInt64().Int64ToDate();
        }

        public void Dispose()
        {
            if (!_leaveInnerStreamOpen)
                _stream.Dispose();
        }
    }
}