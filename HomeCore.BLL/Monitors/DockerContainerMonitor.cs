using HomeCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.BLL.Monitors
{
    public class DockerContainerMonitor : IServiceMonitor
    {
        private readonly IDockerService _dockerService;
        public string MonitorType => "docker_container";

        public DockerContainerMonitor(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async Task<ServiceStatus> CheckAsync(ServiceMonitorConfig config, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(config.ContainerName))
            {
                return new ServiceStatus
                {
                    Name = config.Name,
                    IsHealthy = false,
                    Detail = "ContainerName no está configurado"
                };
            }

            try
            {
                var containers = await _dockerService.GetContainersAsync(all: true, ct);

                var container = containers.FirstOrDefault(c =>
                    string.Equals(c.Name, config.ContainerName, StringComparison.OrdinalIgnoreCase));

                if (container is null)
                {
                    return new ServiceStatus
                    {
                        Name = config.Name,
                        IsHealthy = false,
                        Detail = $"Contenedor '{config.ContainerName}' no encontrado"
                    };
                }

                return new ServiceStatus
                {
                    Name = config.Name,
                    IsHealthy = container.State == "running",
                    Detail = container.Status
                };
            }
            catch (Exception ex)
            {
                return new ServiceStatus
                {
                    Name = config.Name,
                    IsHealthy = false,
                    Detail = $"Error al conectar con Docker: {ex.Message}"
                };
            }
        }
    }
}
