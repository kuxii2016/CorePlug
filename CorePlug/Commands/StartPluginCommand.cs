using Plugin;
using Proxyserver.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Logging;
using Utils.ServerConsole;

namespace Proxyserver.Commands
{
    internal class StartPluginCommand : IConsoleCommand
    {
        private readonly PluginManager manager;

        public StartPluginCommand(PluginManager manager)
        {
            this.manager = manager;
        }

        public string Command => "PStart";

        public string Description =>
            "Starts a stopped plugin by name.";

        public string Usage =>
            "startplugin <pluginName>";

        public IEnumerable<string> Aliases =>
            new[] { "plstart" };

        public bool Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Log.Warning("Usage: startplugin <pluginName>");
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
                Log.Info($"Starting plugin: {plugin.Instance.Name}");

                plugin.Instance.Start(plugin.Context, plugin.Cancellation.Token);

                plugin.State = PluginState.Started;
                plugin.PluginActive = true;

                Log.Info($"Plugin started: {plugin.Instance.Name}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to start plugin: {name}");
                Log.Error(ex);
                return false;
            }
        }
    }
}