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
                return Redirect("http://localhost:5274/Auth/Login");
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("http://localhost:5274/api/v1/main");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Content = "⛔ Доступ заборонено";
            }
            else
            {
                ViewBag.Content = await response.Content.ReadAsStringAsync();
            }

            return View();
        }
    }
}

