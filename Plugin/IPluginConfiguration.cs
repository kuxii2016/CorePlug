namespace Plugin
{
    /// <summary>
    /// Provides methods for loading and saving plugin configuration files.
    /// </summary>
    public interface IPluginConfiguration
    {
        /// <summary>
        /// Loads a configuration file.
        /// If the file does not exist, a new default instance is created and saved.
        /// </summary>
        /// <typeparam name="T">
        /// Configuration object type.
        /// </typeparam>
        /// <param name="fileName">
        /// Configuration file name.
        /// </param>
        /// <returns>
        /// Loaded configuration instance.
        /// </returns>
        T Load<T>(string fileName) where T : new();

        /// <summary>
        /// Saves a configuration object to disk.
        /// </summary>
        /// <typeparam name="T">
        /// Configuration object type.
        /// </typeparam>
        /// <param name="fileName">
        /// Configuration file name.
        /// </param>
        /// <param name="data">
        /// Configuration instance to save.
        /// </param>
        void Save<T>(string fileName, T data);
    }
}