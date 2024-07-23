using System;
using System.IO;

namespace MemoryCleaner
{
    public class MemoryManager : IDisposable
    {
        private bool disposed = false;
        private string logFilePath;

        public MemoryManager(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void InitializeResources()
        {
            // Initialization of resources (if needed)
        }

        public void CleanCache()
        {
            try
            {
                long tempFilesSize = CleanTemporaryFiles();
                CleanBrowserCache();
                LogFinalResult($"Cache cleanup completed. Temporary files cleaned: {tempFilesSize} bytes.");
            }
            catch (Exception ex)
            {
                LogFinalResult($"ERROR - Cache cleanup failed: {ex.Message}");
            }
        }

        private long CleanTemporaryFiles()
        {
            long totalSize = 0;
            string tempPath = Path.GetTempPath();

            if (Directory.Exists(tempPath))
            {
                foreach (var file in Directory.GetFiles(tempPath))
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Exists && !IsSystemFile(fileInfo))
                        {
                            totalSize += fileInfo.Length;
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle file deletion exception but do not log every error
                    }
                }
            }
            return totalSize;
        }

        private void CleanBrowserCache()
        {
            string[] cachePaths =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data", "Default", "Cache"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data", "Default", "Media Cache"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox", "Profiles")
            };

            foreach (string path in cachePaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        DeleteDirectory(path);
                    }
                    catch (Exception ex)
                    {
                        // Handle directory deletion exception but do not log every error
                    }
                }
            }
        }

        private void DeleteDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    foreach (var file in Directory.GetFiles(directoryPath))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            // Handle file deletion exception but do not log every error
                        }
                    }

                    foreach (var dir in Directory.GetDirectories(directoryPath))
                    {
                        DeleteDirectory(dir);
                    }

                    Directory.Delete(directoryPath, true);
                }
            }
            catch (Exception ex)
            {
                // Handle directory deletion exception but do not log every error
            }
        }

        private bool IsSystemFile(FileInfo fileInfo)
        {
            // Simple check to avoid deleting system files (adjust as needed)
            string fileName = fileInfo.Name.ToLower();
            return fileName.Contains("system") || fileName.Contains("config");
        }

        private void LogFinalResult(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose resources if needed
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MemoryManager()
        {
            Dispose(false);
        }
    }
}
