using System.Collections.Generic;
using Utils.Logging;
using Utils.ServerConsole;

namespace Utils.ConsoleCommands
{
    internal class DebugCommand : IConsoleCommand
    {
        public string Command => "Debug";

        public string Description =>
            "Toggles debug logging output.";

        public string Usage =>
            "debug";

        public IEnumerable<string> Aliases =>
            new[] { "dbug" };

        public bool Execute(string[] args)
        {
            Log.isDebugEnabled = !Log.isDebugEnabled;

            Log.Info(
                $"Debug logging is now " +
                $"{(Log.isDebugEnabled ? "ENABLED" : "DISABLED")}.");

            return true;
        }
    }
}