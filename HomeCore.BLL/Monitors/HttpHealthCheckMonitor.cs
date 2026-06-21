using System;
using System.Collections.Generic;
using System.Text;
using HomeCore.Entities;

namespace HomeCore.BLL.Monitors
{
    public class HttpHealthCheckMonitor : IServiceMonitor
    {
        private readonly HttpClient _httpClient;
        public string MonitorType => "http_healthcheck";

        public HttpHealthCheckMonitor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceStatus> CheckAsync(ServiceMonitorConfig config, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(config.Url))
            {
                return new ServiceStatus
                {
                    Name = config.Name,
                    IsHealthy = false,
                    Detail = "URL is not configured"
                };
            }
            try
            {
                using var response = await _httpClient.GetAsync(config.Url, ct);
                return new ServiceStatus
                {
                    Name = config.Name,
                    IsHealthy = response.IsSuccessStatusCode,
                    Detail = $"HTTP {(int)response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceStatus
                {
                    Name = config.Name,
                    IsHealthy = false,
                    Detail = $"Error: {ex.Message}"
                };
            }
        }
    }
}
