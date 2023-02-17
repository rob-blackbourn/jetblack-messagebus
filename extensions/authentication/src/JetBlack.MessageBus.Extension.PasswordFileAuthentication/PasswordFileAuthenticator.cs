using System;
using System.IO;
using System.Linq;
using System.Security;

using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Extension.PasswordFileAuthentication
{
    public class PasswordFileAuthenticator : IAuthenticator
    {
        private readonly FileSystemWatcher _passwordWatcher;
        private JsonPasswordManager _passwordManager;

        private readonly FileSystemWatcher _roleWatcher;
        private JsonRoleManager _roleManager;

        public PasswordFileAuthenticator(string[] args)
        {
            if (args == null || args.Length != 2)
                throw new ArgumentException("Usage: <password-file> <role-file>");

            PasswordFileName = Environment.ExpandEnvironmentVariables(args[0]);

            var passwordFolder = Path.GetDirectoryName(PasswordFileName);
            if (passwordFolder == string.Empty)
                passwordFolder = ".";
            var passwordFileName = Path.GetFileName(PasswordFileName);

            _passwordWatcher = new FileSystemWatcher(passwordFolder, passwordFileName);
            _passwordWatcher.Changed += OnChanged;

            _passwordManager = JsonPasswordManager.Load(PasswordFileName);

            _passwordWatcher.EnableRaisingEvents = true;

            RoleFileName = Environment.ExpandEnvironmentVariables(args[1]);
            var roleFolder = Path.GetDirectoryName(RoleFileName);
            if (roleFolder == string.Empty)
                roleFolder = ".";
            var roleFileName = Path.GetFileName(PasswordFileName);

            _roleWatcher = new FileSystemWatcher(roleFolder, roleFileName);
            _roleWatcher.Changed += OnChanged;

            _roleManager = JsonRoleManager.Load(RoleFileName);

            _roleWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object? sender, FileSystemEventArgs e)
        {
            PasswordManager = JsonPasswordManager.Load(PasswordFileName);
        }

        public string Method => "BASIC";
        public string PasswordFileName { get; }
        public JsonPasswordManager PasswordManager
        {
            get
            {
                lock (_passwordWatcher)
                {
                    return _passwordManager;
                }
            }
            set
            {
                lock (_passwordWatcher)
                {
                    _passwordManager = value;
                }
            }
        }
        public string RoleFileName { get; }
        public JsonRoleManager RoleManager
        {
            get
            {
                lock (_roleWatcher)
                {
                    return _roleManager;
                }
            }
            set
            {
                lock (_roleWatcher)
                {
                    _roleManager = value;
                }
            }
        }

        public AuthenticationResponse Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var connectionString = reader.ReadString();
            var connectionDetails = PasswordFileConnectionDetails.Parse(connectionString);

            if (!PasswordManager.IsValid(connectionDetails.Username, connectionDetails.Password))
                throw new SecurityException();

            var roles = RoleManager.UserFeedRoles.TryGetValue(connectionDetails.Username, out var feedRoles)
                ? feedRoles
                : null;

            return new AuthenticationResponse(
                connectionDetails.Username,
                Method,
                connectionDetails.Impersonating,
                connectionDetails.ForwardedFor,
                connectionDetails.Application,
                feedRoles);
        }
    }
}