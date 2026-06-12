using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartStockManager.API.DTOs;
using PartStockManager.CoreLogic.Services;

namespace PartStockManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = _authService.Login(dto.Username, dto.Password);
                
                if (token == null)
                    return Unauthorized("Invalid username or password.");
                
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login endpoint");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
