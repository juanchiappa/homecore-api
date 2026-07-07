using HomeCore.BLL;
using HomeCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SystemController : ControllerBase
    {
        private readonly ISystemMetricsService _metricsService;
        public SystemController(ISystemMetricsService metricsService)
        {
            _metricsService = metricsService;
        }
        [HttpGet("metrics")]
        [ProducesResponseType(typeof(SystemMetrics), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMetrics(CancellationToken ct = default)
        {
            var metrics = await _metricsService.GetMetricsAsync(ct);
            return Ok(metrics);
        }
        [HttpGet("uptime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUptime()
        {
            var uptime = _metricsService.GetUptime();

            return Ok(new
            {
                uptime.Days,
                uptime.Hours,
                uptime.Minutes,
                uptime.Seconds,
                Formatted = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                TotalSeconds = (long)uptime.TotalSeconds
            });
        }
    }
}
