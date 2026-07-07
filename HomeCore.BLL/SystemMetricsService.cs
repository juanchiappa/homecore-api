using HomeCore.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HomeCore.BLL
{
    public class SystemMetricsService : ISystemMetricsService
    {
        public async Task<SystemMetrics> GetMetricsAsync(CancellationToken ct = default)
        {
            var cpuPercent = await ReadCpuPercentAsync(ct);
            var (memUsed, memTotal) = ReadMemory();
            var (diskUsed, diskTotal) = ReadDisk();
            var uptime = GetUptime();

            var memPercent = memTotal > 0 ? (double)memUsed / memTotal * 100.0 : 0;
            var diskPercent = diskTotal > 0 ? (double)diskUsed / diskTotal * 100.0 : 0;

            return new SystemMetrics
            {
                CpuPercent = Math.Round(cpuPercent, 2),
                MemoryUsedBytes = memUsed,
                MemoryTotalBytes = memTotal,
                MemoryPercent = Math.Round(memPercent, 2),
                DiskUsedBytes = diskUsed,
                DiskTotalBytes = diskTotal,
                DiskPercent = Math.Round(diskPercent, 2),
                Uptime = uptime,
                ReadAt = DateTime.UtcNow
            };
        }

        public TimeSpan GetUptime()
        {
            if (!File.Exists("/proc/uptime"))
                return TimeSpan.Zero;

            var content = File.ReadAllText("/proc/uptime");
            var raw = content.Split(' ')[0];

            return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds)
                ? TimeSpan.FromSeconds(seconds)
                : TimeSpan.Zero;
        }

        private static async Task<double> ReadCpuPercentAsync(CancellationToken ct)
        {
            if (!File.Exists("/proc/stat"))
                return 0.0;

            var (idle1, total1) = ReadCpuStat();
            await Task.Delay(500, ct);
            var (idle2, total2) = ReadCpuStat();

            var totalDelta = total2 - total1;
            var idleDelta = idle2 - idle1;

            if (totalDelta <= 0)
                return 0.0;

            return (1.0 - (double)idleDelta / totalDelta) * 100.0;
        }

        private static (long idle, long total) ReadCpuStat()
        {
            var line = File.ReadLines("/proc/stat").First();
            var values = line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)                   
                .Select(long.Parse)
                .ToArray();

            var idle = values[3] + (values.Length > 4 ? values[4] : 0);
            var total = values.Sum();

            return (idle, total);
        }


        private static (long used, long total) ReadMemory()
        {
            if (!File.Exists("/proc/meminfo"))
                return (0, 0);

            long memTotal = 0, memAvailable = 0;

            foreach (var line in File.ReadLines("/proc/meminfo"))
            {
                if (line.StartsWith("MemTotal:"))
                    memTotal = ParseKilobytes(line);
                else if (line.StartsWith("MemAvailable:"))
                    memAvailable = ParseKilobytes(line);

                if (memTotal > 0 && memAvailable > 0)
                    break;
            }

            return ((memTotal - memAvailable) * 1024, memTotal * 1024);
        }

        private static long ParseKilobytes(string line)
        {
            var valuePart = line
                .Split(':')[1]
                .Replace("kB", string.Empty)
                .Trim();

            return long.TryParse(valuePart, out var result) ? result : 0;
        }

        private static (long used, long total) ReadDisk()
        {
            try
            {
                var drive = DriveInfo.GetDrives()
                    .FirstOrDefault(d => d.IsReady && d.RootDirectory.FullName == "/")
                    ?? DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady);

                if (drive is null)
                    return (0, 0);

                return (drive.TotalSize - drive.TotalFreeSpace, drive.TotalSize);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}
