using System;
using System.IO;
using System.Linq;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    /// <summary>
    /// The null authenticator.
    /// </summary>
    public class NullSspiAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Construct the authenticator.
        /// </summary>
        /// <param name="args">The null authenticator takes no arguments.</param>
        public NullSspiAuthenticator(string[] args)
        {
            // Nothing to do here.
        }

        /// <summary>
        /// The method is "NULL".
        /// </summary>
        public string Method => "NULL";

        /// <summary>
        /// Authentication always succeeds with the user returned as "nobody".
        /// </summary>
        /// <param name="stream">The stream to authenticate on.</param>
        /// <returns>An authentication response for the user "nobody".</returns>
        public AuthenticationResponse Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var connectionString = reader.ReadString();

            var username = Parse(connectionString);
            
            return new AuthenticationResponse(username, Method, null, null, null, null);
        }

        private static string Parse(string connectionString)
        {
            var dict = connectionString.Split(new char[] {';'})
                .Select(part => part.Split(new char[] {'='}, 2))
                .ToDictionary(key => key[0], value => value[1]);

            if (!dict.TryGetValue("Username", out var username))
                throw new ArgumentException("Failed to find \"Username\".");

            return username;            
        }
    }
}