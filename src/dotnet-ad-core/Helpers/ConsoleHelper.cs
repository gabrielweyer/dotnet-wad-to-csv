using System;

namespace DotNet.AzureDiagnostics.Core.Helpers
{
    public class ConsoleHelper
    {
        public static void WriteDebug(string line)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static void WriteError(string line)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(line);
            Console.ResetColor();
        }
    }
}
