#nullable enable

using System;
using System.IO;
using System.Security;
using System.Security.Principal;
using JetBlack.MessageBus.Common.IO;
using Novell.Directory.Ldap;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class LdapAuthenticator : IAuthenticator
    {
        public LdapAuthenticator(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("usage: <ldap-server> <ldap-port>");

            Server = args[0];

            if (int.TryParse(args[1], out var port))
                Port = port;
            else
                throw new ArgumentException("Expected the second argument ldap-port to be an integer");
        }

        public string Server { get; }
        public int Port { get; }

        public GenericIdentity Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var username = reader.ReadString();
            var password = reader.ReadString();

            using (var ldap = new LdapConnection { SecureSocketLayer = true })
            {
                ldap.Connect(Server, Port);
                try
                {
                    ldap.Bind(username, password);
                    if (!ldap.Bound)
                        throw new SecurityException();

                    return new GenericIdentity(username, "LDAP");
                }
                catch
                {
                    throw new SecurityException();
                }
            }
        }
    }
}