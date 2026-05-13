using System.Collections.Generic;
using Utils.Logging;
using Utils.ServerConsole;

namespace Utils.ConsoleCommands
{
    internal class PrettyLogCommand : IConsoleCommand
    {
        public string Command => "PLog";

        public string Description =>
            "Enhances console log formatting for better readability.";

        public string Usage =>
            "prettylog";

        public IEnumerable<string> Aliases =>
            new[] { "plog" };

        public bool Execute(string[] args)
        {
            Log.PrettyLog = !Log.PrettyLog;
            Log.Info($"PrettyLog is now {(Log.PrettyLog ? "enabled" : "disabled")}");
            return true;
        }
    }
}