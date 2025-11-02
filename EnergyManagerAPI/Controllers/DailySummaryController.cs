using EnergyManagerCore.Extentions;
using EnergyManagerCore.Models.DTOs;
using EnergyManagerCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnergyManagerAPI.Controllers
{
    [ApiController]
    [Route("api/houses/{houseId}/summaries")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DailySummaryController : ControllerBase
    {
        private readonly IDailySummaryService _service;

        public DailySummaryController(IDailySummaryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByHouseId(int houseId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            int userId = User.GetUserId();
            var summaries = await _service.GetByHouseIdAsync(houseId, userId, from, to);
            return Ok(summaries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int houseId, int id)
        {
            int userId = User.GetUserId();
            var summary = await _service.GetByIdAsync(id, userId);
            if (summary == null) return NotFound();
            return Ok(summary);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int houseId, [FromBody] CreateDailySummaryDto dto)
        {
            int userId = User.GetUserId();
            var created = await _service.CreateAsync(dto, houseId, userId);
            return CreatedAtAction(nameof(GetById), new { houseId, id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int houseId, int id, [FromBody] DailySummaryDto dto)
        {
            int userId = User.GetUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int houseId, int id)
        {
            int userId = User.GetUserId();
            var success = await _service.DeleteAsync(id, userId);
            if (!success) return NotFound();
            return NoContent();
        }

       
    }

}
