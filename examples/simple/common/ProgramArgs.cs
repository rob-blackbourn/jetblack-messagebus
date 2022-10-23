using System;
using System.Net;

namespace common
{
    public class ProgramArgs
    {
        public readonly string Host;
        public readonly int Port;
        public readonly ConnectionMethod Method;

        private ProgramArgs(string host, int port, ConnectionMethod method)
        {
            Host = host;
            Port = port;
            Method = method;
        }

        public static ProgramArgs Parse(string[] args)
        {
            try
            {
                string host = Dns.GetHostEntry("LocalHost").HostName;
                int? port = null;
                ConnectionMethod method = ConnectionMethod.Plain;

                var argc = 0;
                while (argc < args.Length)
                {
                    switch (args[argc++])
                    {
                        case "--host":
                            host = args[argc++];
                            break;
                        case "--port":
                            port = int.Parse(args[argc++]);
                            break;
                        case "--method":
                            method = Enum.Parse<ConnectionMethod>(args[argc++], true);
                            break;
                    }
                }

                if (!port.HasValue)
                {
                    port = method != ConnectionMethod.Sspi ? 9001 : 9002;
                }

                return new ProgramArgs(
                    host,
                    port.Value,
                    method
                );
            }
            catch
            {
                Console.WriteLine("usage: --host <host> --port <port> --method <Plain|Ssl|Sspi>");
                Environment.Exit(-1);
                throw;
            }
        }

    }

    public enum ConnectionMethod
    {
        Plain,
        Ssl,
        Sspi
    }
}
