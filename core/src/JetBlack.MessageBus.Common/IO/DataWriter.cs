#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JetBlack.MessageBus.Common.IO
{
    /// <summary>
    /// A data writer.
    /// </summary>
    public class DataWriter : IDisposable
    {
        private readonly Stream _stream;
        private readonly bool _leaveInnerStreamOpen;

        /// <summary>
        /// Construct a data writer.
        /// </summary>
        /// <param name="stream">The tsream to write to.</param>
        /// <param name="leaveInnerStreamOpen">If true, leave the inner stream open when disposing.</param>
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
        /// Write an int
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(int value)
        {
            var buf = new[]
            {
                (byte) ((value >> 24) & 0xFF),
                (byte) ((value >> 16) & 0xFF),
                (byte) ((value >> 8) & 0xFF),
                (byte) ((value >> 0) & 0xFF)
            };

            _stream.Write(buf, 0, 4);
        }

        /// <summary>
        /// Write a string
        /// </summary>
        /// <param name="value">The value to write</param>
        public void Write(string? value)
        {
            if (value == null)
            {
                Write(0);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                Write(bytes.Length);
                _stream.Write(bytes, 0, bytes.Length);
            }
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

        public void Write(ISet<int>? value)
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

        public void Write(DataPacket dataPacket)
        {
            Write(dataPacket.Entitlements);
            Write(dataPacket.Data);
        }

        public void Write(DataPacket[]? dataPackets)
        {
            if (dataPackets == null)
                Write(0);
            else
            {
                Write(dataPackets.Length);
                foreach (var dataPacket in dataPackets)
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