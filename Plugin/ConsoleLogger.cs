using Utils.Logging;

namespace Plugin
{
    /// <summary>
    /// Default console-based logger implementation for plugins.
    /// Prepends plugin name to all log messages for better traceability.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly string plugin;

        /// <summary>
        /// Initializes a new instance of the ConsoleLogger class.
        /// </summary>
        /// <param name="plugin">The name of the plugin using this logger.</param>
        public ConsoleLogger(string plugin)
        {
            this.plugin = plugin;
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="text">The message to log.</param>
        public void Info(string text)
        {
            Log.Info($"[{plugin}] {text}");
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="text">The message to log.</param>
        public void Warning(string text)
        {
            Log.Warning($"[{plugin}] {text}");
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="text">The message to log.</param>
        public void Error(string text)
        {
            Log.Error($"[{plugin}] {text}");
        }

        /// <summary>
        /// Logs a debug message (only shown when debug mode is enabled).
        /// </summary>
        /// <param name="text">The message to log.</param>
        public void Debug(string text)
        {
            Log.Debug($"[{plugin}] {text}");
        }
    }
}