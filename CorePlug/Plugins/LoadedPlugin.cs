using Plugin;
using System;
using System.Threading;

namespace Proxyserver.Plugins
{
    /// <summary>
    /// Represents a loaded plugin instance and its runtime state.
    /// </summary>
    public class LoadedPlugin
    {
        /// <summary>
        /// Gets or sets the plugin instance.
        /// </summary>
        public IPlugin Instance { get; set; }

        /// <summary>
        /// Gets or sets the load context used for the plugin assembly.
        /// </summary>
        public PluginLoadContext LoadContext { get; set; }

        /// <summary>
        /// Gets or sets the plugin context containing runtime information and services.
        /// </summary>
        public IPluginContext Context { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token source used to stop plugin tasks or threads.
        /// </summary>
        public CancellationTokenSource Cancellation { get; set; }

        /// <summary>
        /// Gets or sets the current state of the plugin.
        /// </summary>
        public PluginState State { get; set; } = PluginState.Unloadet;

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is currently active.
        /// </summary>
        public bool PluginActive { get; set; } = false;


        public string Name => Instance.Name;

        public Version Version => Instance.Version;
    }
}