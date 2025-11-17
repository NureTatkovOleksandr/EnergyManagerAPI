using EnergyManagerCore.Extentions; // Для User.GetUserId() и других extensions
using EnergyManagerCore.Models.DTOs;
using EnergyManagerCore.Services;
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
        // Внедрите сервис, если нужно (например, IUserService для получения user по ID)
        private readonly IUserService _userService; // Если у вас есть сервис для users
        public MainController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProtectedData() // Сделайте async, если нужно ждать сервис
        {
            // Извлекаем user ID из JWT claims (не из сессии!)
            var userId = User.GetUserId(); // Ваш extension

            // Получаем user из БД или сервиса (не храните в сессии)
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "❌ Пользователь не найден или не авторизован." });
            }

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