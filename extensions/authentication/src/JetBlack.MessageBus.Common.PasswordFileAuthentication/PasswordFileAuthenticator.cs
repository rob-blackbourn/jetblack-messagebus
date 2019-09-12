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
        private readonly FileSystemWatcher _watcher;
        private PasswordManager _manager;

        public PasswordFileAuthenticator(string[] args)
        {
            if (args == null || args.Length != 1)
                throw new ArgumentException("Expected 1 argument");

            FileName = Environment.ExpandEnvironmentVariables(args[0]);

            var directory = Path.GetDirectoryName(FileName);
            var fileName = Path.GetFileName(FileName);

            _watcher = new FileSystemWatcher(directory, fileName);
            _watcher.Changed += OnChanged;

            _manager = PasswordManager.Load(FileName);

            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object? sender, FileSystemEventArgs e)
        {
            Manager = PasswordManager.Load(FileName);
        }

        public string Name => "BASIC";
        public string FileName { get; }
        public PasswordManager Manager
        {
            get
            {
                lock (_watcher)
                {
                    return _manager;
                }
            }
            set
            {
                lock (_watcher)
                {
                    _manager = value;
                }
            }
        }

        public AuthenticationResponse Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var username = reader.ReadString();
            var password = reader.ReadString();
            var impersonating = reader.ReadNullableString();
            var forwardedFor = reader.ReadNullableString();

            if (!Manager.IsValid(username, password))
                throw new SecurityException();

            return new AuthenticationResponse(username, Name, impersonating, forwardedFor);
        }
    }
}