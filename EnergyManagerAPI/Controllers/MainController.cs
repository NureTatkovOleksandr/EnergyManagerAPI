using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            // Тут ты можешь вернуть любую полезную информацию.
            // Пока просто тестовый пример.
            var data = new
            {
                message = "✅ Доступ дозволено! Це приватні дані лише для авторизованих користувачів.",
                timestamp = DateTime.UtcNow,
                user = User.Identity?.Name ?? "невідомий користувач"
            };

            return Ok(data);
        }
    }
}
