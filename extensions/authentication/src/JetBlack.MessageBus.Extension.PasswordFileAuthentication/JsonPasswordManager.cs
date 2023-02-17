using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication
{
    public class JsonPasswordManager
    {
        public JsonPasswordManager()
            : this(new Dictionary<string, Password>())
        {
        }

        public JsonPasswordManager(Dictionary<string, Password> passwords)
        {
            Passwords = passwords;
        }

        public Dictionary<string, Password> Passwords { get; }

        public void Set(string username, string password)
        {
            Passwords[username] = Password.Create(password);
        }

        public void Remove(string username)
        {
            Passwords.Remove(username);
        }

        public bool IsValid(string username, string password)
        {
            return Passwords.TryGetValue(username, out var item) && item.IsValid(password);
        }

        public static JsonPasswordManager Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new JsonPasswordManager();

            using (var reader = File.OpenText(fileName))
            {
                return Load(reader);
            }
        }

        public static JsonPasswordManager Load(StreamReader reader)
        {
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                var passwords = serializer.Deserialize<Dictionary<string, Password>>(jsonReader);
                if (passwords == null)
                    return new JsonPasswordManager();
                else
                    return new JsonPasswordManager(passwords);
            }
        }

        public void Save(string fileName)
        {
            using (var writer = File.CreateText(fileName))
            {
                Save(writer);
            }
        }

        public void Save(StreamWriter writer)
        {
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, Passwords);
            }
        }
    }
}