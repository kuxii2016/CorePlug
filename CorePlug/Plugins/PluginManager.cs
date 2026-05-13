using Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Utils.Logging;
using Utils.ServerConsole;

namespace Proxyserver.Plugins
{
    public class PluginManager
    {
        private readonly List<LoadedPlugin> plugins = new List<LoadedPlugin>();
        private readonly IEventBus eventBus;
        private readonly IServiceProvider services;
        private readonly ConsoleCommandManager commandManager;
        public IReadOnlyList<LoadedPlugin> Plugins => plugins;
        public ConsoleCommandManager CommandManager => commandManager;
        public IEventBus Events => eventBus;


        private const long TickWarningThresholdMs = 10000;
        private const long TickDisableThresholdMs = 10000 * 5;

        public PluginManager(IServiceProvider services = null)
        {
            this.services = services;
            eventBus = new EventBus();
            commandManager = new ConsoleCommandManager();
            Log.Info("Plugin manager initialized");
        }

        public void LoadPlugins(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Log.Info($"Created plugin directory: {path}");
            }

            string[] pluginFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            Log.Info($"Discovered {pluginFiles.Length} plugin assembly(s)");
            foreach (string file in pluginFiles)
            {
                try
                {
                    LoadPlugin(file);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to load plugin assembly: {file}");
                    Log.Error(ex);
                }
            }
            Log.Info($"Plugin loading completed. Total loaded plugins: {plugins.Count}");
        }

        private void LoadPlugin(string file)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Log.Debug($"Loading plugin assembly: {file}");
            PluginLoadContext loadContext = new PluginLoadContext(file);
            Assembly assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(file));
            Type pluginType = assembly.GetTypes().FirstOrDefault(x => typeof(IPlugin).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            if (pluginType == null)
            {
                Log.Warning($"No valid plugin entry point found in assembly: {Path.GetFileName(file)}");
                return;
            }

            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
            PluginContext context = new PluginContext(plugin.Name, eventBus, commandManager, services, Program.ServerVersion);
            LoadedPlugin loaded = new LoadedPlugin
            {
                Instance = plugin,
                LoadContext = loadContext,
                Context = context,
                Cancellation = new CancellationTokenSource(),
                State = PluginState.Loaded
            };

            if (plugin.MinimumHostVersion != null && Program.ServerVersion.CompareTo(plugin.MinimumHostVersion) < 0)
            {
                Log.Warning($"Plugin '{plugin.Name}' not loaded: " + $"requires host version {plugin.MinimumHostVersion} or higher.");
                return;
            }

            plugins.Add(loaded);
            stopwatch.Stop();
            Log.Info(
                $"Plugin loaded successfully: " +
                $"Name='{plugin.Name}', " +
                $"Version='{plugin.Version}', " +
                $"LoadTime={stopwatch.ElapsedMilliseconds}ms");
        }

        public void StartPlugins()
        {
            Log.Info($"Starting {plugins.Count} plugin(s)");
            foreach (LoadedPlugin plugin in plugins)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    Log.Debug($"Starting plugin: {plugin.Instance.Name}");

                    plugin.Instance.Start(plugin.Context, plugin.Cancellation.Token);
                    plugin.State = PluginState.Started;
                    plugin.PluginActive = true;
                    stopwatch.Stop();
                    Log.Info(
                        $"Plugin started successfully: " +
                        $"Name='{plugin.Instance.Name}', " +
                        $"StartupTime={stopwatch.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    plugin.State = PluginState.Error;
                    plugin.PluginActive = false;
                    plugin.Context.Logger.Error(
                        $"Plugin startup failed: " +
                        $"Name='{plugin.Instance.Name}', " +
                        $"StartupTime={stopwatch.ElapsedMilliseconds}ms");
                    plugin.Context.Logger.Error(ex.ToString());
                }
            }
        }

        public void Tick()
        {
            Parallel.ForEach(plugins, plugin =>
            {
                if (plugin.State != PluginState.Started || !plugin.PluginActive)
                    return;

                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    plugin.Instance.OnTick(plugin.Context);

                    stopwatch.Stop();

                    long elapsed = stopwatch.ElapsedMilliseconds;

                    // Prevent TickCount overflow
                    if (plugin.Context.TickCount < long.MaxValue)
                    {
                        plugin.Context.TickCount++;
                    }

                    // Update highest recorded tick time
                    if (elapsed > plugin.Context.HighestTickTime)
                    {
                        plugin.Context.HighestTickTime = elapsed;
                    }

                    // Overflow-safe running average calculation
                    if (plugin.Context.TickCount == 1)
                    {
                        plugin.Context.AverageTickTime = elapsed;
                    }
                    else
                    {
                        plugin.Context.AverageTickTime +=
                            (elapsed - plugin.Context.AverageTickTime)
                            / plugin.Context.TickCount;
                    }

                    // Warn about slow plugin execution
                    if (elapsed >= TickWarningThresholdMs)
                    {
                        plugin.Context.Logger.Warning(
                            $"Plugin tick execution is slow " +
                            $"ExecutionTime={elapsed}ms");
                    }

                    // Disable plugin if execution time exceeds hard limit
                    if (elapsed >= TickDisableThresholdMs)
                    {
                        plugin.State = PluginState.Error;
                        plugin.PluginActive = false;

                        plugin.Context.Logger.Error(
                            $"Plugin disabled due to tick timeout " +
                            $"ExecutionTime={elapsed}ms");
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    plugin.State = PluginState.Error;
                    plugin.PluginActive = false;

                    plugin.Context.Logger.Error(
                        $"Plugin disabled due to unhandled exception during tick execution " +
                        $"ExecutionTime={stopwatch.ElapsedMilliseconds}ms");

                    plugin.Context.Logger.Error(ex.ToString());
                }
            });
        }

        public void StopPlugins()
        {
            Log.Info($"Stopping {plugins.Count} plugin(s)");
            foreach (LoadedPlugin plugin in plugins)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    Log.Debug($"Stopping plugin: {plugin.Name}");
                    plugin.Cancellation.Cancel();
                    plugin.Cancellation.Dispose();
                    plugin.Instance.Stop(plugin.Context);
                    if (eventBus is EventBus bus)
                    {
                        bus.RemovePluginSubscriptions(plugin.Name);
                    }

                    plugin.Instance.Dispose();
                    plugin.State = PluginState.Stopped;
                    plugin.PluginActive = false;
                    stopwatch.Stop();
                    Log.Info(
                        $"Plugin stopped successfully: " +
                        $"Name='{plugin.Name}', " +
                        $"ShutdownTime={stopwatch.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    plugin.Context.Logger.Error(
                        $"Failed to stop plugin: " +
                        $"Name='{plugin.Name}', " +
                        $"ShutdownTime={stopwatch.ElapsedMilliseconds}ms");
                    plugin.Context.Logger.Error(ex.ToString());
                }
            }
        }

        public void UnloadPlugins()
        {
            Log.Info($"Unloading {plugins.Count} plugin(s)");

            foreach (LoadedPlugin plugin in plugins)
            {
                try
                {
                    Log.Debug($"Unloading plugin: {plugin.Instance.Name}");
                    plugin.State = PluginState.Unloadet;
                    plugin.PluginActive = false;
                    plugin.LoadContext.Unload();
                    Log.Info($"Plugin unloaded successfully: {plugin.Instance.Name}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to unload plugin: {plugin.Instance.Name}");
                    Log.Error(ex);
                }
            }

            plugins.Clear();

            Log.Debug("Running garbage collection for unloaded plugins");
            for (int i = 0; i < 3; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Log.Info("Plugin unload process completed");
        }
    }
}