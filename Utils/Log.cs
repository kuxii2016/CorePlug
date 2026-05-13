using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Utils.Logging
{
    public static class Log
    {
        private static readonly object _lock = new object();
        public static string Service { get; set; }
        public static bool PrettyLog { get; set; } = true;
        public static bool isDebugEnabled { get; set; } = false;
        public static bool isFullLog { get; set; } = false;

        public static void Init(string service, bool prettylog = true, bool Debug = false, bool FullLog = false)
        {
            PrettyLog = prettylog;
            Service = service;
            isDebugEnabled = Debug;
            isFullLog = FullLog;
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            string logFile = Path.Combine(logDir, "output.Log");
            if (File.Exists(logFile))
                File.Delete(logFile);
        }

        private static string GetCallerInfo([CallerMemberName] string callerMember = "")
        {
            var stackTrace = new StackTrace(skipFrames: 1, fNeedFileInfo: false);
            foreach (var frame in stackTrace.GetFrames())
            {
                var method = frame.GetMethod();
                if (method == null) continue;

                var declaringType = method.DeclaringType;
                if (declaringType == null) continue;

                string fullName = declaringType.FullName;
                if (fullName == typeof(Log).FullName)
                    continue;

                if (fullName.Contains("+<") ||
                    fullName.StartsWith("System.Runtime.CompilerServices") ||
                    fullName.StartsWith("System.Threading.Tasks"))
                    continue;

                if (isFullLog)
                {
                    if (method.Name.Contains(".ctor"))
                        return $"{fullName}::{declaringType.Name}";
                    else
                        return $"{fullName}::{method.Name}";
                }

                if (method.Name.Contains(".ctor"))
                    return $"{declaringType.Name}";
                else
                    return $"{method.Name}";
            }
            return callerMember;
        }

        private static void WriteToFile(string message)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", "output.Log");
                using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write))
                using (var sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.WriteLine(message);
                }
            }
            catch
            {
                // intentionally empty
            }
        }

        private static void LogMessage(
            string type,
            ConsoleColor color,
            object[] parameters,
            [CallerMemberName] string callerMember = "")
        {
            if (type == "DEBUG" && !isDebugEnabled)
                return;

            lock (_lock)
            {
                string time = DateTime.Now.ToString("HH:mm:ss");
                string caller = GetCallerInfo(callerMember);
                string paramStr = string.Join(", ", parameters ?? Array.Empty<object>());

                string logMessage = $"[{time}] [{type}] {caller}({paramStr})";

                var oldColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"[{time}]");

                Console.ForegroundColor = color;
                Console.Write($"[{type}] ");

                Console.ForegroundColor = ConsoleColor.Gray;

                if (PrettyLog)
                    Console.WriteLine($"{caller}::{paramStr}");
                else
                    Console.WriteLine($"{caller}({paramStr})");

                Console.ForegroundColor = oldColor;

                if (type != "DEBUG")
                {
                    WriteToFile(logMessage);
                }
            }
        }

        public static void RotateLog()
        {
            Info("Starting to Rotate Server Logfiles");
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", "output.Log");
            if (File.Exists(logPath))
            {
                var lines = File.ReadAllLines(logPath);
                File.Delete(logPath);

                string oldDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", "old");
                if (!Directory.Exists(oldDir))
                    Directory.CreateDirectory(oldDir);

                string oldLog = Path.Combine(oldDir, DateTime.Now.ToString("ddMMyyyyHHmmss") + ".Log");
                File.WriteAllLines(oldLog, lines);

                GC.Collect();
                Info("Logfile Rotating Done!");
            }
        }

        public static void PrintBuffer(byte[] buf)
        {
            string text = "\n" + FormatBuffer(buf) + "\n";
            Info(text + "\n");
        }

        private static string FormatBuffer(byte[] buf)
        {
            string bufferResult = "";
            int lineOffset = 0;
            do
            {
                int remainingBytes = buf.Length - lineOffset;
                int remainingBytesInLine = ((lineOffset + 16 > buf.Length - 1) ? remainingBytes : 16);
                string lineResult = $"{lineOffset:X4}";
                for (int columnOffset = 0; columnOffset < remainingBytesInLine; columnOffset++)
                {
                    lineResult += $" {buf[lineOffset + columnOffset]:X2}";
                }
                bufferResult = bufferResult + lineResult + "\n";
                lineOffset += 16;
            }
            while (lineOffset < buf.Length);
            return bufferResult.Trim();
        }

        // Log-Level Methoden
        public static void Debug(params object[] parameters) => LogMessage("DEBUG", ConsoleColor.DarkYellow, parameters);
        public static void Info(params object[] parameters) => LogMessage("INFO", ConsoleColor.Green, parameters);
        public static void Warning(params object[] parameters) => LogMessage("WARNING", ConsoleColor.Yellow, parameters);
        public static void Error(params object[] parameters) => LogMessage("ERROR", ConsoleColor.Red, parameters);
    }
}
