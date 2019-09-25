#nullable enable

using System;
using System.IO;
using System.Security;

using Novell.Directory.Ldap;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class LdapAuthenticator : IAuthenticator
    {
        public LdapAuthenticator(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("usage: <ldap-server> <ldap-port>");

            Server = Environment.ExpandEnvironmentVariables(args[0]);

            if (int.TryParse(args[1], out var port))
                Port = port;
            else
                throw new ArgumentException("Expected the second argument ldap-port to be an integer");
        }

        public string Method => @"LDAP";
        public string Server { get; }
        public int Port { get; }

        public AuthenticationResponse Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var connectionString = reader.ReadString();
            var connectionDetails = LdapConnectionDetails.Parse(connectionString);

            using (var ldap = new LdapConnection { SecureSocketLayer = true })
            {
                ldap.Connect(Server, Port);
                try
                {
                    ldap.Bind(connectionDetails.Username, connectionDetails.Password);
                    if (!ldap.Bound)
                        throw new SecurityException();

                    return new AuthenticationResponse(
                        connectionDetails.Username,
                        Method,
                        connectionDetails.Impersonating,
                        connectionDetails.ForwardedFor);
                }
                catch
                {
                    throw new SecurityException();
                }
            }
        }

    }
}