using HomeCore.BLL;
using HomeCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContainersController : ControllerBase
    {
        private readonly IDockerService _dockerService;
        public ContainersController(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ContainerInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContainers(
    [FromQuery] bool all = false,
    CancellationToken ct = default)
        {
            var containers = await _dockerService.GetContainersAsync(all, ct);
            return Ok(containers);
        }

        [HttpGet("{id}/stats")]
        [ProducesResponseType(typeof(ContainerStats), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContainerStats(string id, CancellationToken ct = default)
        {
            var stats = await _dockerService.GetContainerStatsAsync(id, ct);

            if (stats is null)
                return NotFound(new { message = $"Contenedor '{id}' no encontrado o sin stats disponibles." });

            return Ok(stats);
        }
    }
}
