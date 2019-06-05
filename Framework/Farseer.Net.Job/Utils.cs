using System;

namespace FS.Job
{
    internal class Utils
    {
        public static void WriteLine(string msg, ConsoleColor color)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = consoleColor;
        }
        
        public static void Write(string msg, ConsoleColor color)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(msg);
            Console.ForegroundColor = consoleColor;
        }
    }
}