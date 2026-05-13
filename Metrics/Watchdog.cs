using Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Utils.Logging;

namespace Metrics
{
    internal class Watchdog
    {
        private static IPluginContext context;
        public static Timer _timer;
        public static bool _running;
        private static readonly int IntervalSeconds = 10;
        private static readonly int WindowSize = 360;

        private static readonly List<Snapshot> Snapshots = new List<Snapshot>();

        private static int _warmupCount = 0;
        private static readonly int WarmupNeeded = 5;

        #region Thresholds
        private static int MaxThreads = 200;
        private static double MaxCpuPercent = 80.0;
        private static double MaxMemoryGrowthMBPerMin = 100;
        private static int MaxTcpConnections = 500;
        private static int MaxTimeWaitConnections = 200;
        private static int MaxGen2CollectionsPerMin = 3;
        #endregion

        private class Snapshot
        {
            public DateTime Time;
            public int Threads;
            public long WorkingSet;
            public TimeSpan CpuTime;
            public int TcpConnections;
            public int TcpTimeWait;
            public int UdpListeners;
            public int TcpListeners;
            public int Gen2Collections;
        }

        public static void Start(DiagnostigConfig config, IPluginContext pluginContext)
        {
            context = pluginContext;
            if (!config.Active)
            {
                context.Logger.Info("Watchdog is disabled in configuration");
                return;
            }
            Stop();

            _running = true;
            MaxThreads = config.MaxThreads;
            MaxCpuPercent = config.MaxCpuPercent;
            MaxMemoryGrowthMBPerMin = config.MaxMemoryGrowthMBPerMin;
            MaxTcpConnections = config.MaxTcpConnections;
            MaxTimeWaitConnections = config.MaxTimeWaitConnections;
            MaxGen2CollectionsPerMin = config.MaxGen2CollectionsPerMin;

            _timer = new Timer(Check, null, TimeSpan.Zero, TimeSpan.FromSeconds(IntervalSeconds));

            context.Logger.Info("Watchdog started");
        }

        public static void Check(object state)
        {
            if (!_running)
                return;

            try
            {
                var process = Process.GetCurrentProcess();
                var ip = IPGlobalProperties.GetIPGlobalProperties();
                var tcp = ip.GetActiveTcpConnections();
                var udp = ip.GetActiveUdpListeners();
                var listeners = ip.GetActiveTcpListeners();

                var snap = new Snapshot
                {
                    Time = DateTime.UtcNow,
                    Threads = process.Threads.Count,
                    WorkingSet = process.WorkingSet64,
                    CpuTime = process.TotalProcessorTime,
                    TcpConnections = tcp.Length,
                    TcpTimeWait = tcp.Count(c => c.State == TcpState.TimeWait),
                    UdpListeners = udp.Length,
                    TcpListeners = listeners.Length,
                    Gen2Collections = GC.CollectionCount(2)
                };

                Snapshots.Add(snap);

                if (Snapshots.Count > WindowSize)
                    Snapshots.RemoveAt(0);

                _warmupCount++;

                if (_warmupCount < WarmupNeeded)
                    return;
                EvaluateHealth();
            }
            catch (Exception ex)
            {
                context.Logger.Error("Watchdog error: " + ex.Message);
            }
        }

        private static void EvaluateHealth()
        {
            if (Snapshots.Count < 2)
                return;

            var first = Snapshots.First();
            var last = Snapshots.Last();

            var deltaMs = (last.Time - first.Time).TotalMilliseconds;
            if (deltaMs <= 0)
                return;

            var deltaMin = deltaMs / 60000.0;

            var cpuDelta = (last.CpuTime - first.CpuTime).TotalMilliseconds;

            var cpuPercent = cpuDelta / (Environment.ProcessorCount * deltaMs) * 100.0;

            if (cpuPercent > MaxCpuPercent)
                Log.Warning($"High CPU usage trend: {cpuPercent:F1}%");

            var memDeltaMB = (last.WorkingSet - first.WorkingSet) / 1024.0 / 1024.0;

            var memPerMin = memDeltaMB / deltaMin;

            if (memPerMin > MaxMemoryGrowthMBPerMin)
                Log.Warning($"Memory growth detected: {memPerMin:F1} MB/min");

            if (last.Threads > MaxThreads)
                Log.Warning($"High thread count: {last.Threads}");

            var gen2Delta = last.Gen2Collections - first.Gen2Collections;
            var gen2PerMin = gen2Delta / deltaMin;

            if (gen2PerMin > MaxGen2CollectionsPerMin)
                Log.Warning($"High GC Gen2 pressure: {gen2PerMin:F1}/min");

            var tcpDelta = last.TcpConnections - first.TcpConnections;

            if (last.TcpConnections > MaxTcpConnections || tcpDelta > MaxTcpConnections * 0.2)
            {
                Log.Warning($"High TCP connections: {last.TcpConnections} (Δ {tcpDelta})");
            }

            if (last.TcpTimeWait > MaxTimeWaitConnections)
                Log.Warning($"High TIME_WAIT connections: {last.TcpTimeWait}");

            if (last.TcpListeners == 0)
                Log.Warning("No TCP listeners detected");

            if (last.UdpListeners == 0)
                Log.Warning("No UDP listeners detected");
        }

        public static (double cpuPercent, double memGrowthMBPerMin, int threads, int tcpConnections, int tcpTimeWait, int udpListeners, int tcpListeners, double gen2PerMin) GetCurrentStats()
        {
            if (Snapshots.Count < 2)
                return (0, 0, 0, 0, 0, 0, 0, 0);

            var first = Snapshots.First();
            var last = Snapshots.Last();

            var deltaMs = (last.Time - first.Time).TotalMilliseconds;
            if (deltaMs <= 0)
                deltaMs = 1;

            var deltaMin = deltaMs / 60000.0;

            var cpuDelta = (last.CpuTime - first.CpuTime).TotalMilliseconds;

            var cpuPercent = cpuDelta / (Environment.ProcessorCount * deltaMs) * 100.0;

            var memDeltaMB = (last.WorkingSet - first.WorkingSet) / 1024.0 / 1024.0;

            var memPerMin = memDeltaMB / deltaMin;

            var gen2Delta = last.Gen2Collections - first.Gen2Collections;
            var gen2PerMin = gen2Delta / deltaMin;

            return (cpuPercent, memPerMin, last.Threads, last.TcpConnections, last.TcpTimeWait, last.UdpListeners, last.TcpListeners, gen2PerMin);
        }

        public static string FormatBytes(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (bytes >= GB) return $"{bytes / (double)GB:F2} GB";
            if (bytes >= MB) return $"{bytes / (double)MB:F2} MB";
            if (bytes >= KB) return $"{bytes / (double)KB:F2} KB";
            return $"{bytes} B";
        }

        public static void Stop()
        {
            _running = false;

            try
            {
                _timer?.Change(Timeout.Infinite, Timeout.Infinite);
                _timer?.Dispose();
                _timer = null;

                Snapshots.Clear();
                _warmupCount = 0;

                Log.Info("Server Watchdog stopped");
            }
            catch (Exception ex)
            {
                Log.Error("Watchdog stop error: " + ex.Message);
            }
        }
    }
}