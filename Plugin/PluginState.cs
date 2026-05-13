namespace Plugin
{
    /// <summary>
    /// Represents the current lifecycle state of a plugin.
    /// </summary>
    public enum PluginState
    {
        /// <summary>
        /// The plugin has been loaded into memory but has not been started yet.
        /// </summary>
        Loaded,

        /// <summary>
        /// The plugin is currently running and actively executing logic.
        /// </summary>
        Started,

        /// <summary>
        /// The plugin has been stopped and is no longer executing.
        /// </summary>
        Stopped,

        /// <summary>
        /// The plugin encountered an error and has been disabled.
        /// </summary>
        Error,

        /// <summary>
        /// The plugin has been unloaded from memory and is no longer available.
        /// </summary>
        Unloadet
    }
}