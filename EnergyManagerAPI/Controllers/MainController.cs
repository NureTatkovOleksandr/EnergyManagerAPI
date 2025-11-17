using EnergyManagerCore.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EnergyManagerWeb.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Требует JWT-токен в заголовке Authorization: Bearer <token>
    public class MainController : ControllerBase
    {
        [HttpGet]

        public IActionResult GetProtectedData()
        {
        // Пытаемся достать юзера из сессии
            var userJson = HttpContext.Session.GetString("user");

            if (string.IsNullOrEmpty(userJson))
            {
                return Unauthorized(new { message = "❌ Сессия пуста или пользователь не авторизован." });
            }

            var user = JsonSerializer.Deserialize<UserDto>(userJson);

            var data = new
            {
                message = "✅ Доступ дозволено! Це приватні дані лише для авторизованих користувачів.",
                timestamp = DateTime.UtcNow,
                user
            };

            return Ok(data);
        }
    }
}
