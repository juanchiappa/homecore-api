using HomeCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.BLL
{
    public interface ISystemMetricsService
    {
        Task<SystemMetrics> GetMetricsAsync(CancellationToken ct = default);
        TimeSpan GetUptime();
    }
}
