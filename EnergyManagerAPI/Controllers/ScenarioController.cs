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
    public class ScenarioController : ControllerBase
    {
        private readonly IScenarioService _service;

        public ScenarioController(IScenarioService service) => _service = service;

        [HttpGet("house/{houseId}")]
        public async Task<IActionResult> GetByHouseId(int houseId)
        {
            try
            {
                var userId = User.GetUserId();
                return Ok(await _service.GetByHouseIdAsync(houseId, userId));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.GetUserId();
            var scenario = await _service.GetByIdAsync(id, userId);
            return scenario != null ? Ok(scenario) : NotFound();
        }

        [HttpPost("house/{houseId}")]
        public async Task<IActionResult> Create(int houseId, [FromBody] CreateScenarioDto dto)
        {
            try
            {
                var userId = User.GetUserId();
                var scenario = await _service.CreateAsync(dto, houseId, userId);
                return CreatedAtAction(nameof(Get), new { id = scenario.Id }, scenario);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ScenarioDto dto)
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
    }
}
