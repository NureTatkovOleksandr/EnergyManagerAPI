using EnergyManagerCore.Data;
using EnergyManagerCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Repositories
{
    // Repositories/IScenarioRepository.cs
    public interface IScenarioRepository
    {
        Task<Scenario?> GetByIdAsync(int id);
        Task<IEnumerable<Scenario>> GetByHouseIdAsync(int houseId);
        Task<Scenario> CreateAsync(Scenario scenario);
        Task<Scenario?> UpdateAsync(int id, Scenario scenario);
        Task<bool> DeleteAsync(int id);
    }

    // Repositories/ScenarioRepository.cs
    public class ScenarioRepository : IScenarioRepository
    {
        private readonly ApplicationDbContext _context;

        public ScenarioRepository(ApplicationDbContext context) => _context = context;

        public async Task<Scenario?> GetByIdAsync(int id)
            => await _context.Scenarios.Include(s => s.House).FirstOrDefaultAsync(s => s.Id == id);

        public async Task<IEnumerable<Scenario>> GetByHouseIdAsync(int houseId)
            => await _context.Scenarios.Where(s => s.HouseId == houseId).ToListAsync();

        public async Task<Scenario> CreateAsync(Scenario scenario)
        {
            _context.Scenarios.Add(scenario);
            await _context.SaveChangesAsync();
            return scenario;
        }

        public async Task<Scenario?> UpdateAsync(int id, Scenario scenario)
        {
            var existing = await _context.Scenarios.FindAsync(id);
            if (existing == null) return null;

            existing.Name = scenario.Name;
            existing.Description = scenario.Description;
            existing.Conditions = scenario.Conditions;
            existing.Actions = scenario.Actions;
            existing.IsActive = scenario.IsActive;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var scenario = await _context.Scenarios.FindAsync(id);
            if (scenario == null) return false;

            _context.Scenarios.Remove(scenario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
