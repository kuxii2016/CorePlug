using System;
using System.Collections.Generic;
using Utils.Logging;
using Utils.ServerConsole;

namespace Testplugin
{
    internal class TestCommand : IConsoleCommand
    {
        public string Command => "Test Command";

        public string Description =>
            "Example Command";

        public string Usage =>
            "test";

        public IEnumerable<string> Aliases =>
            new[] { "test" };

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
                $"Hello");

            return true;
        }
    }
}