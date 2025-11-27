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
        private readonly IHouseService _houseService;
        private readonly IDeviceService _deviceService;
        public MainController(IUserService userService, IHouseService houseService, IDeviceService deviceService)
        {
            _userService = userService;
            _houseService = houseService;
            _deviceService = deviceService;
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

        [HttpPost("OptimizeDevices")]
        public async Task<IActionResult> OptimizeDevices()
        {
            var userId = User.GetUserId();
            var houses = await _houseService.GetMyHousesAsync(userId);
            foreach (var house in houses)
            {
                var devices = await _deviceService.GetByHouseIdAsync(house.Id, userId);
                foreach (var device in devices)
                {
                    if (device.Type == "smart_light" || device.Type == "smart_plug")
                    {
                        device.IsActive = false;
                        await _deviceService.UpdateAsync(device.Id, device, userId);
                    }
                }
            }
            return Ok(new { message = "✅ Devices optimized successfully." });
        }

        [HttpPost("ToggleDevice/{deviceId}")]
        public async Task<IActionResult> ToggleDevice(int deviceId)
        {
            var userId = User.GetUserId();
            var device = await _deviceService.GetByIdAsync(deviceId);
            if (device == null)
            {
                return NotFound(new { message = "❌ Device not found." });
            }
            var house = await _houseService.GetByIdAsync(device.HouseId, userId);
            if (house == null || house.UserId != userId)
            {
                return Unauthorized(new { message = "❌ Unauthorized access to device." });
            }
            device.IsActive = !device.IsActive;
            await _deviceService.UpdateAsync(device.Id,device,userId);
            return Ok(new { message = "✅ Device toggled successfully.", isActive = device.IsActive });
        }
    }
}