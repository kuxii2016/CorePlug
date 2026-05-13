using System;
using System.Threading;

namespace Plugin
{
    /// <summary>
    /// Represents a loadable plugin for the host application.
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Gets the unique identifier of the plugin.
        /// </summary>
        Guid PluginId { get; }

        /// <summary>
        /// Gets the display name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the author of the plugin.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets a short description of the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        Version Version { get; }
        string Website { get; }

        /// <summary>
        /// Gets the minimum required host application version.
        /// The plugin should not be loaded if the host version is lower.
        /// </summary>
        Version MinimumHostVersion { get; }

        /// <summary>
        /// Called when the plugin is loaded and started.
        /// </summary>
        /// <param name="context">
        /// The plugin execution context provided by the host.
        /// </param>
        /// <param name="token">
        /// Cancellation token used to notify the plugin that it should stop.
        /// </param>
        void Start(IPluginContext context, CancellationToken token);

        /// <summary>
        /// Called before the plugin is unloaded or stopped.
        /// Plugins should clean up resources and stop background tasks here.
        /// </summary>
        /// <param name="context">
        /// The plugin execution context.
        /// </param>
        void Stop(IPluginContext context);

        /// <summary>
        /// Called every host update tick.
        /// This method should execute quickly and avoid blocking operations.
        /// </summary>
        /// <param name="context">
        /// The plugin execution context.
        /// </param>
        void OnTick(IPluginContext context);

        /// <summary>
        /// Releases all resources used by the plugin.
        /// </summary>
        void Dispose();
    }
}