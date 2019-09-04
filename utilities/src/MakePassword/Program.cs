using System;

using JetBlack.MessageBus.Common.Security.Authentication;

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
            var password = ReadPassword();
            passwordManager.Set(username, password);
            passwordManager.Save(filename);
        }

        private static string ReadPassword()
        {
            var password = "";
            do
            {
                var key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter && !(key.Key == ConsoleKey.U && key.Modifiers == ConsoleModifiers.Control))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.U)
                    {
                        for (var i = 0; i < password.Length; ++i)
                            Console.Write("\b \b");
                        password = "";
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);

            return password;
        }
    }
}
