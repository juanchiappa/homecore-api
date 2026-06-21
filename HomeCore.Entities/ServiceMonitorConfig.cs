using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.Entities
{
    public class ServiceMonitorConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "http_healthcheck" | "docker_container"
        public string? Url { get; set; }
        public string? ContainerName { get; set; }
    }
    public class ServiceStatus
    {
        public string Name { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public string? Detail { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}
