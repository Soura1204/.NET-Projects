using System;
using System.IO;
using System.Reflection;

namespace UserLoginLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UserLoginLogs");
            Directory.CreateDirectory(logDirectory); // Ensure directory exists

            string logPath = Path.Combine(logDirectory, "LoginLogs.txt");
            string logEntry = $"User: {Environment.UserName}, Logged in at: {DateTime.Now}\n";

            File.AppendAllText(logPath, logEntry);
        }
    }
}
