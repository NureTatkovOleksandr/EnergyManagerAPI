using EnergyManagerCore.Models;
using EnergyManagerCore.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EnergyManagerWeb.Controllers.MVC
{
    public class MainController : Controller
    {
        private readonly HttpClient _httpClient;

        public MainController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("jwt");
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("https://energymanagerapi.onrender.com/Auth/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Получаем данные пользователя (как раньше)
            var userResponse = await _httpClient.GetAsync("https://energymanagerapi.onrender.com/api/v1/main");
            if (!userResponse.IsSuccessStatusCode)
            {
                throw new Exception("Response is not OK");
            }

            var userJson = await userResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserDto>>(userJson, options);
            if (apiResponse?.User == null)
            {
                return BadRequest("Не удалось получить данные пользователя.");
            }

            // Теперь загружаем дома на сервере
            var housesResponse = await _httpClient.GetAsync($"https://energymanagerapi.onrender.com/api/v1/House/houses?usrid={apiResponse.User.Id}");
            if (!housesResponse.IsSuccessStatusCode)
            {
                // Можно обработать ошибку, но для простоты продолжим с пустым списком
                housesResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("[]") };
            }

            var housesJson = await housesResponse.Content.ReadAsStringAsync();
            var houses = JsonSerializer.Deserialize<List<HouseDto>>(housesJson, options) ?? new List<HouseDto>();

            // Для каждого дома загружаем устройства и их total measurements
            foreach (var house in houses)
            {
                var devicesResponse = await _httpClient.GetAsync($"https://energymanagerapi.onrender.com/api/v1/Device/byhouse/{house.Id}");
                if (!devicesResponse.IsSuccessStatusCode)
                {
                    house.Devices = new List<DeviceDto>();
                    continue;
                }

                var devicesJson = await devicesResponse.Content.ReadAsStringAsync();
                house.Devices = JsonSerializer.Deserialize<List<DeviceDto>>(devicesJson, options) ?? new List<DeviceDto>();

                foreach (var device in house.Devices)
                {
                    var totalResponse = await _httpClient.GetAsync($"https://energymanagerapi.onrender.com/api/v1/Device/{device.Id}/total");
                    if (totalResponse.IsSuccessStatusCode)
                    {
                        var totalJson = await totalResponse.Content.ReadAsStringAsync();
                        device.TotalMeasurement = JsonSerializer.Deserialize<double>(totalJson, options); // Изменено на int, чтобы соответствовать методу GetTotalMeasurement в DeviceController (возвращает int после округления)
                    }
                    else
                    {
                        device.TotalMeasurement = 0; // Или обработать ошибку
                    }
                }
            }

            // Создаем ViewModel с всеми данными
            var vm = new DashboardViewModel
            {
                UserId = apiResponse.User.Id,
                UserName = apiResponse.User.Username,
                Role = apiResponse.User.Role,
                Message = apiResponse.Message,
                Timestamp = apiResponse.Timestamp,
                Houses = houses // Добавляем список домов с устройствами
            };

            return View(vm);
        }
    }
}