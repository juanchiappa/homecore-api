using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.Entities
{
    public class SystemMetrics
    {
        public double CpuPercent { get; set; }
        public long MemoryUsedBytes { get; set; }
        public long MemoryTotalBytes { get; set; }
        public double MemoryPercent { get; set; }
        public long DiskUsedBytes { get; set; }
        public long DiskTotalBytes { get; set; }
        public double DiskPercent { get; set; }
        public TimeSpan Uptime { get; set; }
        public DateTime ReadAt { get; set; }
    }
}
