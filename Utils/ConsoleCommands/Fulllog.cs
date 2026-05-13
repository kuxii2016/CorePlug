using System.Collections.Generic;
using Utils.Logging;
using Utils.ServerConsole;

namespace Utils.ConsoleCommands
{
    internal class Fulllog : IConsoleCommand
    {
        public string Command => "FLog";

        public string Description =>
            "Toggles detailed logging output including namespaces and caller information.";

        public string Usage =>
            "fulllog";

        public IEnumerable<string> Aliases =>
            new[] { "flog" };

        public bool Execute(string[] args)
        {
            Log.isFullLog = !Log.isFullLog;
            Log.Info("Full Log: " + (Log.isFullLog ? "Enabled" : "Disabled"));
            return true;
        }
    }
}