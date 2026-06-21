using System;
using System.Collections.Generic;
using System.Text;
using HomeCore.Entities;

namespace HomeCore.BLL.Monitors
{
    public interface IServiceMonitor
    {
        string MonitorType { get; }
        Task<ServiceStatus> CheckAsync(ServiceMonitorConfig config, CancellationToken ct = default);
    }
}
