using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace MemoryCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo culture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = culture;

            string logFilePath = "example.txt";
            try
            {
                using (MemoryManager memoryManager = new MemoryManager(logFilePath))
                {
                    memoryManager.InitializeResources();
                    memoryManager.CleanCache();
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                MemoryUtils.DisplayMemoryUsage(logFilePath);
            }
            catch (Exception ex)
            {
                LogFinalResult(logFilePath, $"ERROR - {ex.Message}");
            }
        }

        private static void LogFinalResult(string logFilePath, string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Failed to write to log file: {logEx.Message}");
            }
        }
    }
}
