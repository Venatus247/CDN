using System;

namespace Commons
{
    public static class Logger
    {
        private static readonly object Lock = new object();

        public static void Debug(string text)
        {
            lock (Lock)
            {
                Console.Write($"[{DateTime.Now.ToLongTimeString()}] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"Debug: {text}");
                Console.ResetColor();
            }
        }
        
        public static void Info(string text)
        {
            lock (Lock)
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {text}");
            }
        }

        public static void Warn(string text)
        {
            lock (Lock)
            {
                Console.Write($"[{DateTime.Now.ToLongTimeString()}] ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARN: {text}");
                Console.ResetColor();
            }
        }

        public static void Error(string text)
        {
            lock (Lock)
            {
                Console.Write($"[{DateTime.Now.ToLongTimeString()}] ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {text}");
                Console.ResetColor();
            }
        }

        public static void Exception(Exception e)
        {
            lock (Lock)
            {
                Console.Write($"[{DateTime.Now.ToLongTimeString()}] ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ResetColor();
            }
        }
    }
}