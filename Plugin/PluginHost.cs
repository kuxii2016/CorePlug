using Plugin;
using System.IO;
using System.Text.Json;

namespace PluginHost
{
    /// <summary>
    /// Provides simple JSON-based configuration loading and saving for plugins.
    /// Each plugin can persist and retrieve structured configuration data from disk.
    /// </summary>
    public class PluginConfiguration : IPluginConfiguration
    {
        private readonly string pluginDirectory;

        /// <summary>
        /// Initializes a new instance of the PluginConfiguration class.
        /// </summary>
        /// <param name="pluginDirectory">The directory where plugin configuration files are stored.</param>
        public PluginConfiguration(string pluginDirectory)
        {
            this.pluginDirectory = pluginDirectory;
        }

        /// <summary>
        /// Loads a configuration object from a JSON file.
        /// If the file does not exist, a new default instance is created and saved.
        /// </summary>
        /// <typeparam name="T">The type of configuration object.</typeparam>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <returns>The loaded or newly created configuration instance.</returns>
        public T Load<T>(string fileName)
            where T : new()
        {
            string path = Path.Combine(pluginDirectory, fileName);

            if (!File.Exists(path))
            {
                T instance = new T();
                Save(fileName, instance);
                return instance;
            }

            string json = File.ReadAllText(path);

            T obj = JsonSerializer.Deserialize<T>(json);

            if (obj == null)
                obj = new T();

            return obj;
        }

        /// <summary>
        /// Saves a configuration object to a JSON file with indented formatting.
        /// </summary>
        /// <typeparam name="T">The type of configuration object.</typeparam>
        /// <param name="fileName">The name of the configuration file.</param>
        /// <param name="data">The configuration data to save.</param>
        public void Save<T>(string fileName, T data)
        {
            string path = Path.Combine(pluginDirectory, fileName);

            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(path, json);
        }
    }
}