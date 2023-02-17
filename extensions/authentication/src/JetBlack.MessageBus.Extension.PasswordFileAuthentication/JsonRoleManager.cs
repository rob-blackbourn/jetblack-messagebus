using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using JetBlack.MessageBus.Common.Security.Authentication;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Converters;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication
{
    public class JsonRoleManager
    {
        public JsonRoleManager()
            : this(new Dictionary<string, Dictionary<string, Dictionary<Regex, Permission>>>())
        {
        }

        public JsonRoleManager(Dictionary<string, Dictionary<string , Dictionary<Regex, Permission>>> userFeedRoles)
        {
            UserFeedRoles = userFeedRoles;
        }

        public Dictionary<string, Dictionary<string, Dictionary<Regex, Permission>>> UserFeedRoles { get; }

        public static JsonRoleManager Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new JsonRoleManager();

            using (var reader = File.OpenText(fileName))
            {
                return Load(reader);
            }
        }

        public static JsonRoleManager Load(StreamReader reader)
        {
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                var userFeedRoles = serializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, Permission>>>>(jsonReader);
                if (userFeedRoles == null)
                    return new JsonRoleManager();
                else
                    return new JsonRoleManager(
                        userFeedRoles.ToDictionary(
                            user => user.Key,
                            user => user.Value.ToDictionary(
                                feed => feed.Key,
                                feed => feed.Value.ToDictionary(
                                    host => new Regex(host.Key),
                                    host => host.Value
                                )
                            )
                        )
                    );
            }
        }
    }
}