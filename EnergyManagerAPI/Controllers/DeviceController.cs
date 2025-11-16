using EnergyManagerCore.Extentions;
using EnergyManagerCore.Models.DTOs;
using EnergyManagerCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EnergyManagerAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _service;

        public DeviceController(IDeviceService service) => _service = service;

        [HttpGet("{houseId}")]
        public async Task<IActionResult> GetByHouseId(int houseId)
        {
            var userId = User.GetUserId();  // Extension
            return Ok(await _service.GetByHouseIdAsync(houseId, userId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.GetUserId();
            var device = await _service.GetByIdAsync(id);
            return device != null ? Ok(device) : NotFound();
        }

        [HttpPost("{houseId}")]
        public async Task<IActionResult> Create(int houseId, [FromBody] CreateDeviceDto dto)
        {
            var userId = User.GetUserId();
            var device = await _service.CreateAsync(dto, houseId, userId);
            return CreatedAtAction(nameof(Get), new { id = device.Id }, device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DeviceDto dto)
        {
            var userId = User.GetUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);
            return updated != null ? Ok(updated) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("{deviceId:int}/measurements")]
        public async Task<IActionResult> GetMeasurements(int deviceId)
        {
            var userId = User.GetUserId();

            var measurements = await _service.GetMeasurementsByDeviceIdAsync(deviceId, userId);

            // Якщо пристрій не знайдено або не належить користувачу — 404
            if (measurements == null)
                return NotFound();

            return Ok(measurements);
        }

        /// <summary>
        /// Повертає суму всіх значень value (як int) для вказаного пристрою
        /// GET api/v1/device/{deviceId}/total
        /// </summary>
        [HttpGet("{deviceId:int}/total")]
        public async Task<IActionResult> GetTotalMeasurement(int deviceId)
        {
            var userId = User.GetUserId();

            var total = await _service.GetTotalMeasurementValueAsync(deviceId, userId);

            // Якщо пристрій не існує або не належить користувачу — 404
            // Якщо вимірювань немає — сервіс поверне 0.0, тому просто кастимо до int
            if (total == null)
                return NotFound();

            return Ok((int)Math.Round(total.Value)); // округлення до цілого, як просили int
        }
    }
}
