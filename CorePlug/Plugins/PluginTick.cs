using System;
using System.Diagnostics;
using System.Threading;
using Utils.Logging;

namespace Proxyserver.Plugins
{
    internal class PluginTick
    {
        private static bool running = true;

        public static void ServerTick()
        {
            const int targetFps = 30;
            const double targetMs = 1000.0 / targetFps;

            Stopwatch stopwatch = new Stopwatch();

            while (running)
            {
                stopwatch.Restart();

                try
                {
                    Program.GetPluginManager.Tick();
                }
                catch (Exception ex)
                {
                    Log.Error("Unhandled exception during server tick.", ex);
                }

                stopwatch.Stop();

                double elapsed = stopwatch.Elapsed.TotalMilliseconds;

                if (elapsed > targetMs)
                {
                   // Log.Warning( $"Server tick exceeded target frame time. Expected <= {targetMs:0.00}ms but took {elapsed:0.00}ms.");
                }

                int sleep = (int)(targetMs - elapsed);

                if (sleep > 0)
                    Thread.Sleep(sleep);
            }
        }
    }
}
