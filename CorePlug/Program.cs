using Plugin.Events;
using Proxyserver.Plugins;
using Proxyserver.Register;
using System;
using System.IO;
using System.Threading;
using Utils.Logging;
using Utils.ServerConsole;

namespace Proxyserver
{
    internal class Program
    {
        public static Version ServerVersion = new Version(1, 1, 0);
        private static PluginManager PluginManager;
        public static PluginManager GetPluginManager => PluginManager;
        public static InputConsole ServerConsole { get; private set; }

        static void Main(string[] args)
        {
            Log.Init("Proxy");

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            PluginManager = new PluginManager();
            ServerConsole = new InputConsole(PluginManager.CommandManager);

            while (CommandsRegister.RegisterCommands())
                break;

            ServerConsole.CreateConsole();
            PluginManager.LoadPlugins(path);
            PluginManager.StartPlugins();


            var t = new Thread(new ThreadStart(PluginTick.ServerTick));
            t.IsBackground = true;
            t.Start();

            PluginManager.Events.Publish(new ApplicationReady()
            {
                Message = "Application Fully Loadet",
                Time = DateTime.UtcNow
            });
        }


    }
}
