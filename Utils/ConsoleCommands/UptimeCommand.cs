using System;
using System.Collections.Generic;
using Utils.Logging;
using Utils.ServerConsole;

namespace Utils.ConsoleCommands
{
    public class UptimeCommand : IConsoleCommand
    {
        public string Command => "Uptime";

        public string Description =>
            "Shows the current server and system uptime.";

        public string Usage =>
            "uptime";

        public IEnumerable<string> Aliases =>
            new[] { "up" };

        public bool Execute(string[] args)
        {
            TimeSpan runtime =
                DateTime.Now -
                System.Diagnostics.Process
                    .GetCurrentProcess()
                    .StartTime;

            DateTime systemStart =
                DateTime.Now.AddMilliseconds(
                    -Environment.TickCount64);

            TimeSpan systemUptime =
                DateTime.Now - systemStart;

            Log.Info(
                $"Gameserver Uptime: " +
                $"{runtime.Days}d " +
                $"{runtime.Hours}h " +
                $"{runtime.Minutes}m " +
                $"{runtime.Seconds}s");

            Log.Info(
                $"System Uptime: " +
                $"{systemUptime.Days}d " +
                $"{systemUptime.Hours}h " +
                $"{systemUptime.Minutes}m " +
                $"{systemUptime.Seconds}s");

            return true;
        }
    }
}