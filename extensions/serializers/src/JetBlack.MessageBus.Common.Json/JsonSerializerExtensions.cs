#nullable enable

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace JetBlack.MessageBus.Common.Json
{
    /// <summary>
    /// Extension methods for serialization
    /// </summary>
    public static class JsonSerializerExtensions
    {
        /// <summary>
        /// Serialize an object to a string.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A json string representing the object.</returns>
        public static string Serialize(this JsonSerializer serializer, object value)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonWriter, value);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Serialize an object to a stream.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="stream">The stream to serialize the object to.</param>
        public static void Serialize(this JsonSerializer serializer, object value, Stream stream)
        {
            using (var stringWriter = new StreamWriter(stream))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonWriter, value);
                }
            }
        }

        /// <summary>
        /// Deserialize an object from a text string.
        /// </summary>
        /// <param name="serializer">The json serializer</param>
        /// <param name="text">The string to be deserialized.</param>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(this JsonSerializer serializer, string text)
        {
            using (var reader = new StringReader(text))
            {
                return serializer.Deserialize<T>(reader);
            }
        }

        /// <summary>
        /// Deserialize an object from a stream.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="stream">The stream from which the object should be deserialized.</param>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(this JsonSerializer serializer, Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return serializer.Deserialize<T>(reader);
            }
        }

        /// <summary>
        /// Deserialize an object from a text reader.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="textReader">The text reader.</param>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(this JsonSerializer serializer, TextReader textReader)
        {
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        /// <summary>
        /// Deserialize object from a string.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="text">The string from which the object should be deserialized.</param>
        /// <param name="objectType">The type of the object to be deserialized.</param>
        /// <returns>The deserialized object</returns>
        public static object Deserialize(this JsonSerializer serializer, string text, Type objectType)
        {
            using (var reader = new StringReader(text))
            {
                return serializer.Deserialize(reader, objectType);
            }
        }

        /// <summary>
        /// Deserialize an object from a stream.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="stream">The stream from which the object should be deserialized.</param>
        /// <param name="objectType">The type of the object to be deserialized.</param>
        /// <returns>The deserialized object</returns>
        public static object Deserialize(this JsonSerializer serializer, Stream stream, Type objectType)
        {
            using (var reader = new StreamReader(stream))
            {
                return serializer.Deserialize(reader, objectType);
            }
        }

        /// <summary>
        /// Deserialize an object from a text reader.
        /// </summary>
        /// <param name="serializer">The json serializer.</param>
        /// <param name="textReader">The text reader from which the object should be deserialized.</param>
        /// <param name="objectType">The type of the object to be deserialized.</param>
        /// <returns>The deserialized object</returns>
        public static object Deserialize(this JsonSerializer serializer, TextReader textReader, Type objectType)
        {
            using (var reader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize(reader, objectType);
            }
        }

        /// <summary>
        /// Deserialize directly from a URI.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized</typeparam>
        /// <param name="serializer">The serializer</param>
        /// <param name="uri">The uri</param>
        /// <param name="timeout">A timeout</param>
        /// <returns>The deserialized document.</returns>
        public static async Task<T> DeserializeAsync<T>(this JsonSerializer serializer, Uri uri, TimeSpan? timeout = null)
        {
            using (var client = new HttpClient())
            {
                if (timeout.HasValue)
                    client.Timeout = timeout.Value;

                using (var response = await client.GetAsync(uri))
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            using (var jsonReader = new JsonTextReader(streamReader))
                            {
                                return serializer.Deserialize<T>(jsonReader);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize json from a file
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="path">The file path</param>
        /// <param name="fileMode">The file mode</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromFile<T>(this JsonSerializer serializer, string path, FileMode fileMode = FileMode.Open)
        {
            using (var stream = new FileStream(path, fileMode, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }
            }
        }

        /// <summary>
        /// Serialize an object to a file.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="serializer">The serializer to user</param>
        /// <param name="content">The object to serialize</param>
        /// <param name="path">The path to the file.</param>
        /// <param name="fileMode">The file mode</param>
        public static void SerializeToFile<T>(this JsonSerializer serializer, T content, string path, FileMode fileMode = FileMode.CreateNew)
        {
            using (var stream = new FileStream(path, fileMode, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        serializer.Serialize(jsonWriter, content);
                    }
                }
            }
        }
    }
}
