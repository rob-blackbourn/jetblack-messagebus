using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using JetBlack.MessageBus.Common.Security.Authentication;
using System.Linq;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication
{
    public class JsonRoleManager
    {
        public JsonRoleManager()
            : this(new Dictionary<string, Dictionary<string, Dictionary<string, Permission>>>())
        {
        }

        public JsonRoleManager(Dictionary<string, Dictionary<string , Dictionary<string, Permission>>> userFeedRoles)
        {
            UserFeedRoles = userFeedRoles;
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, Permission>>> UserFeedRoles { get; }

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
                var userFeedRoles = serializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, PermissionConfig>>>>(jsonReader);
                if (userFeedRoles == null)
                    return new JsonRoleManager();
                else
                    return new JsonRoleManager(
                        userFeedRoles.ToDictionary(
                            user => user.Key,
                            user => user.Value.ToDictionary(
                                feed => feed.Key,
                                feed => feed.Value.ToDictionary(
                                    host => host.Key,
                                    host => new Permission(
                                        host.Value.Allow?.Aggregate(Role.None, (aggregate, role) => aggregate | role) ?? Role.None,
                                        host.Value.Deny?.Aggregate(Role.None, (aggregate, role) => aggregate | role) ?? Role.None)))));
            }
        }
    }
}