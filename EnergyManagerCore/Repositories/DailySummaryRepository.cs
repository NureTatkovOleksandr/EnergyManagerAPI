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
    // Repositories/IDailySummaryRepository.cs
    public interface IDailySummaryRepository
    {
        Task<DailySummary?> GetByIdAsync(int id);
        Task<IEnumerable<DailySummary>> GetByHouseIdAsync(int houseId, DateTime? from = null, DateTime? to = null);
        Task<DailySummary> CreateAsync(DailySummary summary);
        Task<DailySummary?> UpdateAsync(int id, DailySummary summary);
        Task<bool> DeleteAsync(int id);
    }

    // Repositories/DailySummaryRepository.cs
    public class DailySummaryRepository : IDailySummaryRepository
    {
        private readonly ApplicationDbContext _context;

        public DailySummaryRepository(ApplicationDbContext context) => _context = context;

        public async Task<DailySummary?> GetByIdAsync(int id)
            => await _context.DailySummaries.Include(ds => ds.House).FirstOrDefaultAsync(ds => ds.Id == id);

        public async Task<IEnumerable<DailySummary>> GetByHouseIdAsync(int houseId, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.DailySummaries.Where(ds => ds.HouseId == houseId)
                .OrderByDescending(ds => ds.Date);  // **DESC для дашборду**
            if (from.HasValue) query = (IOrderedQueryable<DailySummary>)query.Where(ds => ds.Date >= from.Value);
            if (to.HasValue) query = (IOrderedQueryable<DailySummary>)query.Where(ds => ds.Date <= to.Value);
            return await query.ToListAsync();
        }

        public async Task<DailySummary> CreateAsync(DailySummary summary)
        {
            _context.DailySummaries.Add(summary);
            await _context.SaveChangesAsync();
            return summary;
        }

        public async Task<DailySummary?> UpdateAsync(int id, DailySummary summary)
        {
            var existing = await _context.DailySummaries.FindAsync(id);
            if (existing == null) return null;

            existing.TotalConsumption = summary.TotalConsumption;
            existing.PeakHour = summary.PeakHour;
            existing.Savings = summary.Savings;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var summary = await _context.DailySummaries.FindAsync(id);
            if (summary == null) return false;

            _context.DailySummaries.Remove(summary);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
