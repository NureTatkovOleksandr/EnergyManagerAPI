using EnergyManagerCore.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace EnergyManagerWeb.Controllers.MVC
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://energymanagerapi.onrender.com/api/v1/"); // URL твоего API
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginDto());

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email і пароль обов'язкові");

            var response = await _httpClient.PostAsJsonAsync("auth/login", dto);
            if (!response.IsSuccessStatusCode)
                return Unauthorized("Невірний email або пароль");

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            HttpContext.Session.SetString("jwt", result!.Token);

            return Json(new { token = result.Token });
        }


        [HttpGet]
        public IActionResult Register() => View(new RegisterDto());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", dto);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Email вже існує");
                return View(dto);
            }

            return RedirectToAction("Login");
        }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
    }
}
