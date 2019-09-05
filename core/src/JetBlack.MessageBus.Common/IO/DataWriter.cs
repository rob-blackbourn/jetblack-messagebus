#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace JetBlack.MessageBus.Common.IO
{
    public class DataWriter : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveInnerStreamOpen;

        public DataWriter(Stream stream, bool leaveInnerStreamOpen = true)
        {
            _stream = stream;
            _leaveInnerStreamOpen = leaveInnerStreamOpen;
        }

        /// <summary>
        /// Write a boolean
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(bool value)
        {
            _stream.WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Write a byte
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        /// Write a character
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(char value)
        {
            _stream.Write(value.GetBytes(), 0, 2);
        }

        /// <summary>
        /// Write an int
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(int value)
        {
            _stream.Write(value.GetBytes(), 0, 4);
        }

        /// <summary>
        /// Write a long
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(long value)
        {
            _stream.Write(value.GetBytes(), 0, 8);
        }

        /// <summary>
        /// Write a short
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(short value)
        {
            _stream.Write(value.GetBytes(), 0, 2);
        }

        /// <summary>
        /// Write an unsigned short to a stream
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(ushort value)
        {
            _stream.Write(PortableBitConverter.GetBytes(value), 0, 2);
        }

        /// <summary>
        /// Write a float
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(float value)
        {
            _stream.Write(value.GetBytes(), 0, 4);
        }

        /// <summary>
        /// Write a double
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(double value)
        {
            Write(BitConverter.DoubleToInt64Bits(value));
        }

        /// <summary>
        /// Write a string
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            Write(bytes.Length);
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Write a date
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(DateTime value)
        {
            Write(value.DateToInt64());
        }

        /// <summary>
        /// Write an ip address to a stream.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(IPAddress value)
        {
            byte[] address = value.GetAddressBytes();
            Write(address.Length);
            _stream.Write(address, 0, address.Length);
        }

        /// <summary>
        /// Write a byte array
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(byte[]? value)
        {
            if (value == null)
                Write(0);
            else
            {
                Write(value.Length);
                _stream.Write(value, 0, value.Length);
            }
        }

        /// <summary>
        /// Write an array of byte arrays
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(byte[][] value)
        {
            if (value == null)
                Write(0);
            else
            {
                Write(value.Length);
                foreach (var t in value)
                    Write(t);
            }
        }

        /// <summary>
        /// Write an array of stings to a stream
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(string[]? value)
        {
            if (value == null)
                Write(0);
            else
            {
                Write(value.Length);
                foreach (var t in value)
                    Write(t);
            }
        }

        public void Write(int[]? value)
        {
            if (value == null)
                Write(0);
            else
            {
                Write(value.Length);
                foreach (var t in value)
                    Write(t);
            }
        }

        public void Write(HashSet<int>? value)
        {
            if (value == null)
                Write(0);
            else
            {
                Write(value.Count);
                foreach (var t in value)
                    Write(t);
            }
        }

        /// <summary>
        /// Write a guid.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(Guid value)
        {
            var buf = value.ToByteArray();
            _stream.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Write an array of guids.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(Guid[]? value)
        {
            if (value == null)
                Write(0);
            else
            {
                Write(value.Length);
                foreach (var guid in value)
                    Write(guid);
            }
        }

        public void Write(BinaryDataPacket dataPacket)
        {
            Write(dataPacket.Entitlements);
            Write(dataPacket.Data);
        }

        public void Write(BinaryDataPacket[]? data)
        {
            if (data == null)
                Write(0);
            else
            {
                Write(data.Length);
                foreach (var dataPacket in data)
                    Write(dataPacket);
            }
        }

        public void Dispose()
        {
            if (!_leaveInnerStreamOpen)
                _stream.Dispose();
        }
    }
}