using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.Entities
{
    public class ContainerStats
    {
        public string ContainerId { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public double CpuPercent { get; set; }
        public long MemoryUsageBytes { get; set; }
        public long MemoryLimitBytes { get; set; }
        public double MemoryPercent { get; set; }
        public long NetworkRxBytes { get; set; }
        public long NetworkTxBytes { get; set; }
        public DateTime ReadAt { get; set; }
    }
}
