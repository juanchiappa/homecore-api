using HomeCore.BLL;
using HomeCore.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HomeCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            if (response is null)
                return Unauthorized(new { message = "Usuario o contraseña inválidos." });

            return Ok(response);
        }
    }
}
