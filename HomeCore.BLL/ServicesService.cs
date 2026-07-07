using HomeCore.BLL.Monitors;
using HomeCore.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace HomeCore.BLL
{
    public class ServicesService : IServicesService
    {
        private readonly ServiceMonitorFactory _factory;
        private readonly string _configPath;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ServicesService(ServiceMonitorFactory factory, IConfiguration configuration)
        {
            _factory = factory;
            _configPath = configuration["ServicesConfigPath"] ?? "config/services.json";
        }

        public async Task<List<ServiceStatus>> CheckAllAsync(CancellationToken ct = default)
        {
            if (!File.Exists(_configPath))
            {
                return new List<ServiceStatus>
            {
                new ServiceStatus
                {
                    Name      = "configuration",
                    IsHealthy = false,
                    Detail    = $"Archivo de configuración no encontrado: '{_configPath}'. " +
                                "Creá config/services.json basándote en services.example.json."
                }
            };
            }

            try
            {
                var json = await File.ReadAllTextAsync(_configPath, ct);
                var config = JsonSerializer.Deserialize<ServicesConfig>(json, _jsonOptions);

                if (config?.Services is null || config.Services.Count == 0)
                    return new List<ServiceStatus>();

                return await _factory.CheckAllAsync(config.Services, ct);
            }
            catch (JsonException ex)
            {
                return new List<ServiceStatus>
            {
                new ServiceStatus
                {
                    Name      = "configuration",
                    IsHealthy = false,
                    Detail    = $"Error al parsear services.json: {ex.Message}"
                }
            };
            }
        }
    }
}
