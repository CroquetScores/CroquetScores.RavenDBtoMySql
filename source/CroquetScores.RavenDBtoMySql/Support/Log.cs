using System;
using System.IO;

namespace CroquetScores.RavenDBtoMySql.Support
{
    internal class Log
    {
        public static readonly string FileName = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "CroquetScores.RavenDbToMySql.txt");

        private static DateTime _started;

        public static void Warning(string message)
        {
            WriteToFile($"WARNING: {message}");
            WriteToConsole(message, ConsoleColor.DarkMagenta);
        }

        private static void WriteToConsole(string message, ConsoleColor foregroundColor)
        {
            var originalForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);
            Console.ForegroundColor = originalForegroundColor;
        }

        public static void Progress(string message)
        {
            WriteToConsole(message);
        }

        private static void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }

        public static void Statistic(string message)
        {
            WriteToFile($"STATISTIC: {message}");
            WriteToConsole(message);
        }

        public static void Start()
        {
            _started = DateTime.Now;
            WriteToFile($"Started: {_started:s}");
        }

        public static void Finish()
        {
            WriteToFile($"Finished: {DateTime.Now:s}");
            WriteToFile($"Took: {(DateTime.Now - _started).TotalMinutes:N2} minutes");
        }

        private static void WriteToFile(string message)
        {
            File.AppendAllText(FileName, message + Environment.NewLine);
        }

        public static void Debug(string message)
        {
            Console.WriteLine($"DEBUG: {message}");
        }

        public static void Error(string message)
        {
            WriteToFile($"ERROR: {message}");
            WriteToConsole(message, ConsoleColor.Red);
        }
    }
}