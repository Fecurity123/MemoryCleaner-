using System;
using System.IO;

namespace MemoryCleaner
{
    public static class MemoryUtils
    {
        public static void DisplayMemoryUsage(string logFilePath)
        {
            long totalMemory = GC.GetTotalMemory(false);
            string message = $"Memory usage after cleanup: {totalMemory / (1024 * 1024)} MB";
            Console.WriteLine(message);

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write memory usage to log file: {ex.Message}");
            }
        }
    }
}
