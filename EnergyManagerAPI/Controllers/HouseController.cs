using EnergyManagerCore.Models.DTOs;
using EnergyManagerCore.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnergyManagerAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class HouseController : ControllerBase
    {
        private readonly IHouseService _service;

        public HouseController(IHouseService service) => _service = service;

        // **GET /api/v1/house** - Мої будинки
        [HttpGet("houses")]
        public async Task<IActionResult> GetMyHouses(int usrid)
        {
            var result = await _service.GetMyHousesAsync(usrid);

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices(int houseId)
        {
            var result = await _service.GetHomeDevices(houseId);

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var house = await _service.GetByIdAsync(id, userId);
            return house != null ? Ok(house) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHouseDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var house = await _service.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(Get), new { id = house.Id }, house);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HouseDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var updated = await _service.UpdateAsync(id, dto, userId);
            return updated != null ? Ok(updated) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var deleted = await _service.DeleteAsync(id, userId);
            return deleted ? NoContent() : NotFound();
        }
    }
}
