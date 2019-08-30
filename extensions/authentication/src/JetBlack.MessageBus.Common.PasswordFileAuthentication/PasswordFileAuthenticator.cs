#nullable enable

using System;
using System.IO;
using System.Security;
using System.Security.Principal;
using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class PasswordFileAuthenticator : IAuthenticator
    {

        public PasswordFileAuthenticator(string[] args)
        {
            if (args == null || args.Length != 1)
                throw new ArgumentException("Expected 1 argument");

            FileName = args[0];
            Manager = PasswordManager.Load(FileName);
        }

        public string FileName { get; }
        public PasswordManager Manager { get; }

        public GenericIdentity Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var username = reader.ReadString();
            var password = reader.ReadString();

            if (!Manager.IsValid(username, password))
                throw new SecurityException();

            return new GenericIdentity(username, "BASIC");
        }
    }
}