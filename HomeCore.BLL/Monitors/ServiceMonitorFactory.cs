using HomeCore.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.BLL.Monitors
{
    public class ServiceMonitorFactory
    {
        private readonly IEnumerable<IServiceMonitor> _monitors;
        public ServiceMonitorFactory(IEnumerable<IServiceMonitor> monitors)
        {
            _monitors = monitors;
        }

        public IServiceMonitor? GetMonitor(string type) => _monitors.FirstOrDefault(m => m.MonitorType == type);
        public async Task<List<ServiceStatus>> CheckAllAsync(IEnumerable<ServiceMonitorConfig> configs, CancellationToken ct = default)
        {
            var results = new List<ServiceStatus>();

            foreach (var config in configs)
            {
                var monitor = GetMonitor(config.Type);
                if (monitor is null)
                {
                    results.Add(new ServiceStatus
                    {
                        Name = config.Name,
                        IsHealthy = false,
                        Detail = $"No monitor found for type '{config.Type}'"
                    });
                    continue;
                }
                results.Add(await monitor.CheckAsync(config, ct));
            }
            return results;
        }
    }
}
