using EnergyManagerCore.Models.DTOs;
using EnergyManagerCore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnergyManagerAPI.Controllers
{
    // Controllers/AuthController.cs (✅ Заміни весь!)
    [ApiController]
    [Route("api/v1/[controller]")]
    [AllowAnonymous]  // Гостьовий доступ
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        // POST /api/v1/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return BadRequest("Email вже існує");

            // ✅ Авто-логін: повертаємо токен
            return Ok(result);  // result.Token
        }

        // POST /api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized("Невірний email або пароль");

            return Ok(result);  // result.Token
        }
    }
}
