using System.IO;
using System.Text.Json;

namespace Plugin
{
    /// <summary>
    /// Provides JSON-based configuration storage for plugins.
    /// Supports loading and saving strongly typed configuration objects.
    /// </summary>
    public class JsonConfiguration : IPluginConfiguration
    {
        private readonly string basePath;
        /// <summary>
        /// Initializes a new instance of the JsonConfiguration class.
        /// </summary>
        /// <param name="basePath">The directory where configuration files are stored.</param>
        public JsonConfiguration(string basePath)
        {
            this.basePath = basePath;
        }

        /// <summary>
        /// Loads a configuration file as a strongly typed object.
        /// If the file does not exist, a default instance is created and saved.
        /// </summary>
        /// <typeparam name="T">The type of configuration object.</typeparam>
        /// <param name="fileName">The configuration file name.</param>
        /// <returns>The loaded configuration instance, or a new default instance if missing or invalid.</returns>
        public T Load<T>(string fileName) where T : new()
        {
            string path = Path.Combine(basePath, fileName);

            if (!File.Exists(path))
            {
                T instance = new T();

                Save(fileName, instance);

                return instance;
            }

            string json = File.ReadAllText(path);
            var result = JsonSerializer.Deserialize<T>(json);
            return result ?? new T();
        }

        /// <summary>
        /// Saves a configuration object to a JSON file using indented formatting.
        /// </summary>
        /// <typeparam name="T">The type of configuration object.</typeparam>
        /// <param name="fileName">The configuration file name.</param>
        /// <param name="data">The data to serialize and save.</param>
        public void Save<T>(string fileName, T data)
        {
            string path = Path.Combine(basePath, fileName);

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
    }
}