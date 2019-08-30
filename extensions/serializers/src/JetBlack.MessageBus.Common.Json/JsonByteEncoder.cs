#nullable enable

using System;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Common.Json
{
    public class JsonByteEncoder : IByteEncoder
    {
        public JsonSerializer Serializer { get; }

        public JsonByteEncoder()
        {
            Serializer = JsonSerializer.Create();
        }

        public object? Decode(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var json = Encoding.UTF8.GetString(bytes);

            var frame = Serializer.Deserialize<JsonFrame>(json);
            var type = Type.GetType(frame.MetaData);
            if (type == null)
                throw new ApplicationException($"Unable to deserialize unknown type \"{frame.MetaData}\"");

            var obj = Serializer.Deserialize(frame.Data ?? string.Empty, type);

            return obj;
        }

        public byte[]? Encode(object? data)
        {
            if (data == null)
                return null;

            var frame = new JsonFrame
            {
                MetaData = data.GetType().AssemblyQualifiedName ?? "null",
                Data = Serializer.Serialize(data)
            };

            var json = Serializer.Serialize(frame);

            return Encoding.UTF8.GetBytes(json);
        }
    }

    public class JsonFrame
    {
        public string? MetaData { get; set; }
        public string? Data { get; set; }
    }
}
