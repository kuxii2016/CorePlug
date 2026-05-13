using Proxyserver.Plugins;
using System.Collections.Generic;
using Utils.Logging;
using Utils.ServerConsole;

namespace Proxyserver.Commands
{
    internal class PluginsCommand : IConsoleCommand
    {
        private readonly PluginManager manager;

        public PluginsCommand(PluginManager manager)
        {
            this.manager = manager;
        }

        public string Command => "Plugins";

        public string Description =>
            "Lists all loaded plugins and their current state.";

        public string Usage =>
            "plugins";

        public IEnumerable<string> Aliases =>
            new[] { "pl" };

        public bool Execute(string[] args)
        {
            var plugins = manager.Plugins;

            Log.Info("=== Loaded Plugins ===");

            foreach (var p in plugins)
            {
                Log.Info(
                    $"{p.Instance.Name} " +
                    $"| Version: {p.Instance.Version} - {p.Instance.PluginId} " +
                    $"| State: {p.State} " +
                    $"| Active: {p.PluginActive}");
            }

            Log.Info($"Total plugins: {plugins.Count}");

            return true;
        }
    }
}