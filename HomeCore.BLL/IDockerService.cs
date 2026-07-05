using System;
using System.Collections.Generic;
using System.Text;
using HomeCore.Entities;

namespace HomeCore.BLL
{
    public interface IDockerService
    {
        Task<List<ContainerInfo>> GetContainersAsync(bool all = false, CancellationToken ct = default);
        Task<ContainerStats?> GetContainerStatsAsync(string containerId, CancellationToken ct = default);
    }
}
