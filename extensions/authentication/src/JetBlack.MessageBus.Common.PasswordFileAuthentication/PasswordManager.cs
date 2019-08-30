#nullable enable

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class PasswordManager
    {
        public PasswordManager()
            : this(new Dictionary<string, Password>())
        {
        }

        public PasswordManager(Dictionary<string, Password> passwords)
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

        public static PasswordManager Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new PasswordManager();

            using (var reader = File.OpenText(fileName))
            {
                return Load(reader);
            }
        }

        public static PasswordManager Load(StreamReader reader)
        {
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                var passwords = serializer.Deserialize<Dictionary<string, Password>>(jsonReader);
                return new PasswordManager(passwords);
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