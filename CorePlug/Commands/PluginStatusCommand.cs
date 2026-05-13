using Proxyserver.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Logging;
using Utils.ServerConsole;

namespace Proxyserver.Commands
{
    internal class PluginStatusCommand : IConsoleCommand
    {
        private readonly PluginManager manager;

        public PluginStatusCommand(PluginManager manager)
        {
            this.manager = manager;
        }

        public string Command => "PState";

        public string Description =>
            "Shows detailed status information for all loaded plugins.";

        public string Usage =>
            "pluginstatus [pluginName]";

        public IEnumerable<string> Aliases =>
            new[] { "pstatus", "ps" };

        public bool Execute(string[] args)
        {
            var plugins = manager.Plugins;
            if (args.Length > 0)
            {
                string name = string.Join(" ", args);
                var plugin = plugins.FirstOrDefault(p => p.Instance.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (plugin == null)
                {
                    Log.Warning($"Plugin not found: {name}");
                    return false;
                }

                Status(plugin);
                return true;
            }

            foreach (var plugin in plugins)
            {
                Status(plugin);
            }

            Log.Info($"Total plugins: {plugins.Count}");
            return true;
        }

        private void Status(LoadedPlugin plugin)
        {
            Log.Info($"Name: {plugin.Instance.Name} Version: {plugin.Instance.Version} State: {plugin.State} ");
            Log.Info($"Active: {plugin.PluginActive} Thread: {(plugin.Cancellation.IsCancellationRequested ? "Cancelled" : "Running")}");
        }
    }
}