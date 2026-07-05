using System;
using System.Collections.Generic;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using HomeCore.Entities;

namespace HomeCore.BLL
{
    public class DockerService : IDockerService
    {
        private readonly DockerClient _dockerClient;

        public DockerService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }

        public async Task<List<ContainerInfo>> GetContainersAsync(bool all = false, CancellationToken ct = default)
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters { All = all }, ct);

            return containers.Select(c => new ContainerInfo
            {
                Id = c.ID,
                Name = c.Names.FirstOrDefault()?.TrimStart('/') ?? string.Empty,
                Image = c.Image,
                State = c.State,
                Status = c.Status,
                Created = c.Created
            }).ToList();
        }

        public async Task<ContainerStats?> GetContainerStatsAsync(string containerId, CancellationToken ct = default)
        {
            ContainerStatsResponse? statsResponse = null;
            var tcs = new TaskCompletionSource<ContainerStatsResponse>();

            var progress = new Progress<ContainerStatsResponse>(s => tcs.TrySetResult(s));

            await _dockerClient.Containers.GetContainerStatsAsync(
                containerId,
                new ContainerStatsParameters { Stream = false },
                progress,
                ct);

            statsResponse = await tcs.Task;

            if (statsResponse is null)
                return null;

            var cpuPercent = CalculateCpuPercent(statsResponse);
            var memUsage = (long)statsResponse.MemoryStats.Usage;
            var memLimit = (long)statsResponse.MemoryStats.Limit;
            var memPercent = memLimit > 0 ? (double)memUsage / memLimit * 100.0 : 0;
            var networkRx = statsResponse.Networks?.Values.Sum(n => (long)n.RxBytes) ?? 0;
            var networkTx = statsResponse.Networks?.Values.Sum(n => (long)n.TxBytes) ?? 0;

            return new ContainerStats
            {
                ContainerId = containerId,
                ContainerName = statsResponse.Name.TrimStart('/'),
                CpuPercent = Math.Round(cpuPercent, 2),
                MemoryUsageBytes = memUsage,
                MemoryLimitBytes = memLimit,
                MemoryPercent = Math.Round(memPercent, 2),
                NetworkRxBytes = networkRx,
                NetworkTxBytes = networkTx,
                ReadAt = statsResponse.Read
            };
        }

        //Helpers 
        private static double CalculateCpuPercent(ContainerStatsResponse stats)
        {
            var cpuDelta = (double)(stats.CPUStats.CPUUsage.TotalUsage - stats.PreCPUStats.CPUUsage.TotalUsage);
            var systemDelta = (double)(stats.CPUStats.SystemUsage - stats.PreCPUStats.SystemUsage);

            if (systemDelta <= 0 || cpuDelta < 0)
                return 0.0;

            var numCpus = stats.CPUStats.OnlineCPUs;
            if (numCpus == 0 && stats.CPUStats.CPUUsage.PercpuUsage is not null)
                numCpus = (ulong)stats.CPUStats.CPUUsage.PercpuUsage.Count;
            if (numCpus == 0)
                numCpus = 1;

            return cpuDelta / systemDelta * numCpus * 100.0;
        }
    }
}
