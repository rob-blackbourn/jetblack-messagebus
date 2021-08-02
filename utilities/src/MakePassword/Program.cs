using System;

using JetBlack.MessageBus.Common;
using JetBlack.MessageBus.Extension.PasswordFileAuthentication;

namespace MakePassword
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: MakePassword <filename> <username>");
                Environment.Exit(1);
            }

            var filename = args[0];
            var username = args[1];

            var passwordManager = PasswordManager.Load(filename);
            Console.Write("Enter password: ");
            var password = ConsoleHelper.ReadPassword();
            passwordManager.Set(username, password);
            passwordManager.Save(filename);
        }
    }
}
