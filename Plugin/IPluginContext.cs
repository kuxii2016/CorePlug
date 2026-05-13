using System;
using Utils.ServerConsole;

namespace Plugin
{
    /// <summary>
    /// Provides access to the host application's systems and services for plugins.
    /// </summary>
    public interface IPluginContext
    {
        ILogger Logger { get; }
        /// <summary>
        /// Gets the current version of the host application.
        /// </summary>
        Version AppVersion { get; }

        /// <summary>
        /// Gets the directory assigned to the plugin.
        /// Plugins can use this directory to store configuration,
        /// cache, save files, or other persistent data.
        /// </summary>
        string PluginDirectory { get; }

        /// <summary>
        /// Gets the global event bus used to subscribe and publish events.
        /// </summary>
        IEventBus Events { get; }

        /// <summary>
        /// Gets the global service provider used to access
        /// host application systems and shared services.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Provides methods for loading and saving plugin configuration files.
        /// </summary>
        IPluginConfiguration Configuration { get; }
        ConsoleCommandManager Commands { get; }

        /// <summary>
        /// Registers an action that will be executed every host tick/update cycle.
        /// </summary>
        /// <param name="action">
        /// The action to execute during each tick.
        /// </param>
        void RegisterTick(Action action);
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
    }
}