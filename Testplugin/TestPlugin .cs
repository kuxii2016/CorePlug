using Plugin;
using Plugin.Events;
using System;
using System.Threading;
using Utils.Logging;

namespace Testplugin
{
    /// <summary>
    /// Represents the plugin configuration file.
    /// </summary>
    public class TestConfig
    {
        /// <summary>
        /// Gets or sets the displayed server name.
        /// </summary>
        public string ServerName { get; set; } = "My Server";
        /// <summary>
        /// Gets or sets the maximum allowed player count.
        /// </summary>
        public int MaxPlayers { get; set; } = 32;

        /// <summary>
        /// Gets or sets a value indicating whether debug mode is enabled.
        /// </summary>
        public bool Debug { get; set; } = false;
    }

    /// <summary>
    /// Example plugin implementation demonstrating:
    /// configuration handling,
    /// command registration,
    /// event subscriptions,
    /// logging,
    /// and plugin lifecycle management.
    /// </summary>
    /// <remarks>
    /// Plugin lifecycle:
    /// Start()
    ///     ↓
    /// OnTick()
    ///     ↓
    /// Stop()
    ///     ↓
    /// Dispose()
    /// </remarks>
    public class TestPlugin : IPlugin
    {
        private TestConfig config;
        /// <summary>
        /// Gets the unique plugin identifier.
        /// </summary>
        public Guid PluginId => Guid.NewGuid();
        /// <summary>
        /// Gets the plugin display name.
        /// </summary>
        public string Name => "Developer Example Plugin";
        /// <summary>
        /// Gets the plugin author name.
        /// </summary>
        public string Author => "Kuxii";
        /// <summary>
        /// Gets the plugin description.
        /// </summary>
        public string Description => "Official Example Plugin";
        /// <summary>
        /// Gets the plugin project website.
        /// </summary>
        public string Website => "";

        /// <summary>
        /// Gets the current plugin version.
        /// </summary>
        public Version Version => new Version(1, 2, 1);
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
            var rnd = new Random().Next(0, 50000);
            Thread.Sleep(100 + rnd); // Simulate work
            Log.Info($"Plugin Times: {context.AverageTickTime}, {context.HighestTickTime}, {context.TickCount}");
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

            // Load plugin configuration
            config = context.Configuration.Load<TestConfig>("config.json");

            context.Logger.Info("ServerName: " + config.ServerName);

            // Example configuration modification
            config.MaxPlayers = 64;

            // Save updated configuration
            context.Configuration.Save("config.json", config);

            // Register plugin commands
            context.Commands.RegisterCommand(Name, new TestCommand());

            // Subscribe to events
            context.Events.Subscribe<ApplicationReady>(Name, ServerStarted);

            context.Logger.Info("Plugin started successfully");
        }

        /// <summary>
        /// Handles the application ready event.
        /// </summary>
        /// <param name="event">The application ready event data.</param>
        private void ServerStarted(ApplicationReady @event)
        {
            context.Logger.Info("Message Event: " + @event.Message);
        }

        /// <summary>
        /// Called before the plugin is stopped.
        /// Used to unregister commands, events, and release runtime resources.
        /// </summary>
        /// <param name="context">The plugin runtime context.</param>
        public void Stop(IPluginContext context)
        {
            context.Logger.Info("Plugin Stopped");
            context.Commands.UnregisterPluginCommands(Name);
        }

        /// <summary>
        /// Called when the plugin is permanently unloaded.
        /// Used for final cleanup and resource disposal.
        /// </summary>
        public void Dispose()
        {
            // Called when plugin is permanently unloaded
            // 1. Ensure no lingering debug references
            config = null;
            // 2. If you had timers / threads -> stop here
            // 3. Optional logging (only if logger still valid)
            // (avoid heavy operations here)
        }
    }
}