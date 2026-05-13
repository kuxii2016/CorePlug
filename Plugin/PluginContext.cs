using System;
using System.IO;
using Utils.ServerConsole;

namespace Plugin
{
    /// <summary>
    /// Provides the runtime context for a plugin instance.
    /// This includes logging, configuration, event system, commands, and service access.
    /// </summary>
    public class PluginContext : IPluginContext
    {
        /// <summary>
        /// Logger instance scoped to the plugin.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Plugin configuration handler for reading and writing persistent data.
        /// </summary>
        public IPluginConfiguration Configuration { get; }

        /// <summary>
        /// Event bus used for publishing and subscribing to system events.
        /// </summary>
        public IEventBus Events { get; }

        /// <summary>
        /// Global console command manager for registering plugin commands.
        /// </summary>
        public ConsoleCommandManager Commands { get; }

        /// <summary>
        /// Shared service provider for dependency injection access.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Absolute directory path assigned to this plugin for file storage.
        /// </summary>
        public string PluginDirectory { get; }

        /// <summary>
        /// Version of the host application the plugin is running on.
        /// </summary>
        public Version AppVersion { get; }
        /// <summary>
        /// Gets or sets the average execution time of all plugin ticks in milliseconds.
        /// </summary>
        public long AverageTickTime { get; set; }

        /// <summary>
        /// Gets or sets the highest recorded plugin tick execution time in milliseconds.
        /// </summary>
        public long HighestTickTime { get; set; }

        /// <summary>
        /// Gets or sets the total number of executed plugin ticks.
        /// </summary>
        public long TickCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the PluginContext class.
        /// </summary>
        /// <param name="pluginName">The unique name of the plugin.</param>
        /// <param name="eventBus">Shared event bus instance.</param>
        /// <param name="commandManager">Global command manager.</param>
        /// <param name="services">Shared service provider.</param>
        public PluginContext(string pluginName, IEventBus eventBus, ConsoleCommandManager commandManager, IServiceProvider services, Version appVersion)
        {
            PluginDirectory = Path.Combine("Plugins", pluginName);
            Directory.CreateDirectory(Path.Combine("Config", pluginName));
            Logger = new ConsoleLogger(pluginName);
            Configuration = new JsonConfiguration(Path.Combine("Config", pluginName));
            Events = eventBus;
            Services = services;
            Commands = commandManager;
            AppVersion = appVersion;
        }

        /// <summary>
        /// Registers a tick callback that should be executed on every server tick.
        /// </summary>
        /// <param name="action">The action to execute each tick.</param>
        public void RegisterTick(Action action)
        {
            // Implementation pending
        }
    }
}