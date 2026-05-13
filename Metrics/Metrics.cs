using Plugin;
using Plugin.Events;
using System;
using System.Threading;
using Utils.Logging;

namespace Metrics
{
    public class Metrics : IPlugin
    {
        private DiagnostigConfig config;
        /// <summary>
        /// Gets the unique plugin identifier.
        /// </summary>
        public Guid PluginId => Guid.NewGuid();
        /// <summary>
        /// Gets the plugin display name.
        /// </summary>
        public string Name => "Metrics";
        /// <summary>
        /// Gets the plugin author name.
        /// </summary>
        public string Author => "Kuxii";
        /// <summary>
        /// Gets the plugin description.
        /// </summary>
        public string Description => "server ressourcen watchdog";
        /// <summary>
        /// Gets the plugin project website.
        /// </summary>
        public string Website => "";
        /// <summary>
        /// Gets the current plugin version.
        /// </summary>
        public Version Version => new Version(1, 0, 0);
        /// <summary>
        /// Gets the minimum required host version.
        /// </summary>
        public Version MinimumHostVersion => new Version(1, 0, 0);
        private IPluginContext context;

        /// <summary>
        /// Called every server tick while the plugin is active.
        /// </summary>
        /// <param name="context">The plugin runtime context.</param>
        public void OnTick(IPluginContext context)
        {
        }

        /// <summary>
        /// Called when the plugin is loaded and started.
        /// Used to initialize configuration, commands, events, and services.
        /// </summary>
        /// <param name="context">The plugin runtime context.</param>
        /// <param name="token">Cancellation token used for shutdown requests.</param>
        public void Start(IPluginContext context, CancellationToken token)
        {
            this.context = context;
            config = context.Configuration.Load<DiagnostigConfig>("Diagnostic.json");
            context.Commands.RegisterCommand(Name, new MetricsCommand());
            Watchdog.Start(config, context);
        }

        /// <summary>
        /// Called before the plugin is stopped.
        /// Used to unregister commands, events, and release runtime resources.
        /// </summary>
        /// <param name="context">The plugin runtime context.</param>
        public void Stop(IPluginContext context)
        {
            context.Commands.UnregisterPluginCommands(Name);
            Watchdog.Stop();
        }

        /// <summary>
        /// Called when the plugin is permanently unloaded.
        /// Used for final cleanup and resource disposal.
        /// </summary>
        public void Dispose()
        {
        }
    }
}