using System;

using JetBlack.MessageBus.Common.Security.Authentication;

namespace MakePasswords
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: {Environment.GetCommandLineArgs()[0]} <filename>");
                Environment.Exit(1);
            }

            var filename = args[0];

            var passwords = PasswordManager.Load(filename);

            Console.WriteLine("Add an empty user or password to quit");

            while (true)
            {
                Console.Write("User: ");
                var user = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(user)) break;

                Console.Write("Password: ");
                var password = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(password)) break;

                passwords.Set(user, password);
            }

            Console.Write("Save file [y/N]: ");
            var response = Console.ReadLine();
            if (response.ToLowerInvariant() == "y")
                passwords.Save(filename);
        }
    }
}
