using EnergyManagerCore.Models;
using EnergyManagerCore.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

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

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://energymanagerapi.onrender.com/api/v1/main");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Respone is not OK") ; // или бросать ошибку
            }

            var json = await response.Content.ReadAsStringAsync();
            var user = System.Text.Json.JsonSerializer.Deserialize<UserDto>(json); // DTO с Id

            var vm = new DashboardViewModel
            {
                UserId = user.Id
            };

            return View(vm);
        }

    }
}

