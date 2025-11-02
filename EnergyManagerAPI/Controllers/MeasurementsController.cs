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
    public class MeasurementController : ControllerBase
    {
        private readonly IMeasurementService _service;

        public MeasurementController(IMeasurementService service) => _service = service;

        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetByDeviceId(int deviceId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            try
            {
                var userId = User.GetUserId();
                return Ok(await _service.GetByDeviceIdAsync(deviceId, userId, from, to));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var userId = User.GetUserId();
            var measurement = await _service.GetByIdAsync(id, userId);
            return measurement != null ? Ok(measurement) : NotFound();
        }

        [HttpPost("device/{deviceId}")]
        public async Task<IActionResult> Create(int deviceId, [FromBody] CreateMeasurementDto dto)
        {
            try
            {
                var userId = User.GetUserId();
                var measurement = await _service.CreateAsync(dto, deviceId, userId);
                return CreatedAtAction(nameof(Get), new { id = measurement.Id }, measurement);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] MeasurementDto dto)
        {
            var userId = User.GetUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);
            return updated != null ? Ok(updated) : NotFound();
        }

        // ... (GetByDeviceId, Get, Create, Update - з попереднього)

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = User.GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);
            return deleted ? NoContent() : NotFound();
        }
    }
}