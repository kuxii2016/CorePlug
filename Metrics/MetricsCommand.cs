using System;
using System.Collections.Generic;
using System.Text;
using Utils.Logging;
using Utils.ServerConsole;

namespace Metrics
{
    internal class MetricsCommand : IConsoleCommand
    {
        public string Command => "metrics";

        public string Description =>
            "Show Application Usage Infos";

        public string Usage =>
            "metrics";

        public IEnumerable<string> Aliases =>
            new[] { "top" };

        public bool Execute(string[] args)
        {
            var data = Watchdog.GetCurrentStats();
            Log.Info($"CPU: {data.cpuPercent:F2}% | Mem Growth: {data.memGrowthMBPerMin:F2} MB/min | Threads: {data.threads} | Gen2 Collections/min: {data.gen2PerMin:F2}");
            return true;
        }
    }
}