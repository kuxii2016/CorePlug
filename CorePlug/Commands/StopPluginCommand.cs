using Plugin;
using Proxyserver.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Logging;
using Utils.ServerConsole;

namespace Proxyserver.Commands
{
    internal class StopPluginCommand : IConsoleCommand
    {
        private readonly PluginManager manager;

        public StopPluginCommand(PluginManager manager)
        {
            this.manager = manager;
        }

        public string Command => "PStop";

        public string Description =>
            "Stops a running plugin by name.";

        public string Usage =>
            "stopplugin <pluginName>";

        public IEnumerable<string> Aliases =>
            new[] { "plstop" };

        public bool Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Log.Warning("Usage: stopplugin <pluginName>");
                return false;
            }

            string name = string.Join(" ", args);

            var plugin = manager.Plugins
                .FirstOrDefault(p =>
                    p.Instance.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (plugin == null)
            {
                Log.Warning($"Plugin not found: {name}");
                return false;
            }

            try
            {
                Log.Info($"Stopping plugin: {plugin.Instance.Name}");

                plugin.Cancellation.Cancel();
                plugin.Instance.Stop(plugin.Context);

                plugin.State = PluginState.Stopped;
                plugin.PluginActive = false;

                Log.Info($"Plugin stopped: {plugin.Instance.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to stop plugin: {name}");
                Log.Error(ex);
                return false;
            }
        }
    }
}