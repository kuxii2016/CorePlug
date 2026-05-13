namespace Plugin
{
    /// <summary>
    /// Provides a logging interface for plugins to output messages
    /// with different severity levels.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message used for general runtime information.
        /// </summary>
        /// <param name="text">The message to log.</param>
        void Info(string text);

        /// <summary>
        /// Logs a warning message indicating a potential issue or unexpected behavior.
        /// </summary>
        /// <param name="text">The message to log.</param>
        void Warning(string text);

        /// <summary>
        /// Logs an error message indicating a failure or exception.
        /// </summary>
        /// <param name="text">The message to log.</param>
        void Error(string text);

        /// <summary>
        /// Logs a debug message used for development and diagnostic purposes.
        /// </summary>
        /// <param name="text">The message to log.</param>
        void Debug(string text);
    }
}