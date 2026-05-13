using System;
using System.Collections.Generic;
using System.Text;

namespace Metrics
{
    public class DiagnostigConfig
    {
        public bool Active { get;  set; } = false;
        public int MaxThreads { get;  set; } = 200;
        public double MaxCpuPercent { get;  set; } = 80.0;
        public long MaxMemoryGrowthMBPerMin { get;  set; } = 100;
        public int MaxTcpConnections { get;  set; } = 500;
        public int MaxTimeWaitConnections { get;  set; } = 200;
        public int MaxGen2CollectionsPerMin { get;  set; } = 3;
    }
}
