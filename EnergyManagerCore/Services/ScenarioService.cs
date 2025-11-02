using EnergyManagerCore.Models;
using EnergyManagerCore.Models.DTOs;
using EnergyManagerCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Services
{
    // Services/IScenarioService.cs
    public interface IScenarioService
    {
        Task<IEnumerable<ScenarioDto>> GetByHouseIdAsync(int houseId, int userId);
        Task<ScenarioDto?> GetByIdAsync(int id, int userId);
        Task<ScenarioDto> CreateAsync(CreateScenarioDto dto, int houseId, int userId);
        Task<ScenarioDto?> UpdateAsync(int id, ScenarioDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }

    // Services/ScenarioService.cs
    public class ScenarioService : IScenarioService
    {
        private readonly IScenarioRepository _repo;
        private readonly IHouseService _houseService;

        public ScenarioService(IScenarioRepository repo, IHouseService houseService)
        {
            _repo = repo;
            _houseService = houseService;
        }

        public async Task<IEnumerable<ScenarioDto>> GetByHouseIdAsync(int houseId, int userId)
        {
            var house = await _houseService.GetByIdAsync(houseId, userId);
            if (house == null) throw new UnauthorizedAccessException();
            var scenarios = await _repo.GetByHouseIdAsync(houseId);
            return scenarios.Select(s => new ScenarioDto
            {
                Id = s.Id,
                HouseId = s.HouseId,
                Name = s.Name,
                Description = s.Description,
                Conditions = s.Conditions,
                Actions = s.Actions,
                IsActive = s.IsActive
            });
        }

        public async Task<ScenarioDto?> GetByIdAsync(int id, int userId)
        {
            var scenario = await _repo.GetByIdAsync(id);
            if (scenario == null || scenario.House.UserId != userId) return null;
            return new ScenarioDto { /* map */ };
        }

        public async Task<ScenarioDto> CreateAsync(CreateScenarioDto dto, int houseId, int userId)
        {
            var house = await _houseService.GetByIdAsync(houseId, userId);
            if (house == null) throw new UnauthorizedAccessException();
            var scenario = new Scenario
            {
                HouseId = houseId,
                Name = dto.Name,
                Description = dto.Description,
                Conditions = dto.Conditions,
                Actions = dto.Actions,
                IsActive = dto.IsActive
            };
            var created = await _repo.CreateAsync(scenario);
            return new ScenarioDto { /* map */ };
        }

        public async Task<ScenarioDto?> UpdateAsync(int id, ScenarioDto dto, int userId)
        {
            var scenario = new Scenario
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                Conditions = dto.Conditions,
                Actions = dto.Actions,
                IsActive = dto.IsActive
            };
            var updated = await _repo.UpdateAsync(id, scenario);
            return updated != null ? new ScenarioDto { /* map */ } : null;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var scenario = await _repo.GetByIdAsync(id);
            if (scenario == null || scenario.House.UserId != userId) return false;
            return await _repo.DeleteAsync(id);
        }
    }
}
