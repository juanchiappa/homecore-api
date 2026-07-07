using HomeCore.BLL;
using HomeCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ServiceStatus>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServices(CancellationToken ct = default)
        {
            var statuses = await _servicesService.CheckAllAsync(ct);
            return Ok(statuses);
        }
    }
}
