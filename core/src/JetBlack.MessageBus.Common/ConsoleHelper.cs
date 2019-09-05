using System;

namespace JetBlack.MessageBus.Common
{
    public static class ConsoleHelper
    {
        public static string ReadPassword()
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