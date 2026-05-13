using System.Collections.Generic;
using Utils.Logging;
using Utils.ServerConsole;

namespace Utils.ConsoleCommands
{
    internal class HelpCommand : IConsoleCommand
    {
        private readonly ConsoleCommandManager manager;

        public HelpCommand(ConsoleCommandManager manager)
        {
            this.manager = manager;
        }

        public string Command => "Help";

        public string Description =>
            "Displays a list of all available console commands.";

        public string Usage =>
            "help";

        public IEnumerable<string> Aliases =>
            new[] { "?" };

        public bool Execute(string[] args)
        {
            var commands = manager.GetCommands();
            foreach (var cmd in commands)
            {
                string usage = $"CMD: {string.Join(", ", cmd.Aliases)}, {cmd.Command} | {cmd.Description}";
                Log.Info(usage);
            }
            Log.Info($"Total commands: {commands.Count}");

            return true;
        }
    }
}