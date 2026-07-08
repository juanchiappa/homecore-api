using HomeCore.BLL.Monitors;
using HomeCore.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.Tests
{
    public class ServiceMonitorFactoryTests
    {
        [Fact]
        public async Task CheckAllAsync_KnownType_ReturnsStatus()
        {
            // Arrange
            var monitorMock = new Mock<IServiceMonitor>();
            monitorMock.Setup(m => m.MonitorType).Returns("http_healthcheck");
            monitorMock
                .Setup(m => m.CheckAsync(It.IsAny<ServiceMonitorConfig>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceStatus { Name = "Jellyfin", IsHealthy = true, Detail = "HTTP 200" });

            var factory = new ServiceMonitorFactory(new[] { monitorMock.Object });

            var configs = new List<ServiceMonitorConfig>
        {
            new ServiceMonitorConfig
            {
                Name = "Jellyfin",
                Type = "http_healthcheck",
                Url  = "http://jellyfin:8096/health"
            }
        };

            // Act
            var results = await factory.CheckAllAsync(configs);

            // Assert
            Assert.Single(results);
            Assert.True(results[0].IsHealthy);
            Assert.Equal("Jellyfin", results[0].Name);
        }

        [Fact]
        public async Task CheckAllAsync_UnknownType_ReturnsUnhealthy()
        {
            // Arrange
            var factory = new ServiceMonitorFactory(Enumerable.Empty<IServiceMonitor>());

            var configs = new List<ServiceMonitorConfig>
        {
            new ServiceMonitorConfig
            {
                Name = "Servicio raro",
                Type = "tipo_inexistente"
            }
        };

            // Act
            var results = await factory.CheckAllAsync(configs);

            // Assert
            Assert.Single(results);
            Assert.False(results[0].IsHealthy);
            Assert.Contains("tipo_inexistente", results[0].Detail);
        }

        [Fact]
        public void GetMonitor_ExistingType_ReturnsMonitor()
        {
            // Arrange
            var monitorMock = new Mock<IServiceMonitor>();
            monitorMock.Setup(m => m.MonitorType).Returns("http_healthcheck");

            var factory = new ServiceMonitorFactory(new[] { monitorMock.Object });

            // Act
            var monitor = factory.GetMonitor("http_healthcheck");

            // Assert
            Assert.NotNull(monitor);
        }

        [Fact]
        public void GetMonitor_NonExistingType_ReturnsNull()
        {
            // Arrange
            var factory = new ServiceMonitorFactory(Enumerable.Empty<IServiceMonitor>());

            // Act
            var monitor = factory.GetMonitor("tipo_inexistente");

            // Assert
            Assert.Null(monitor);
        }
    }
}
