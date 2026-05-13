using System.Reflection;
using System.Runtime.Loader;

namespace Proxyserver.Plugins
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver resolver;

        public PluginLoadContext(string pluginPath)
            : base(true)
        {
            resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string path = resolver.ResolveAssemblyToPath(assemblyName);

            if (path != null)
                return LoadFromAssemblyPath(path);

            return null;
        }
    }
}