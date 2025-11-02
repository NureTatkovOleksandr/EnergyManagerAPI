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
    // Repositories/IMeasurementRepository.cs
    public interface IMeasurementRepository
    {
        Task<Measurement?> GetByIdAsync(long id);
        Task<IEnumerable<Measurement>> GetByDeviceIdAsync(int deviceId, DateTime? from = null, DateTime? to = null, int limit = 100);
        Task<Measurement> CreateAsync(Measurement measurement);
        Task<Measurement?> UpdateAsync(long id, Measurement measurement);
        Task<bool> DeleteAsync(long id);
    }

    // Repositories/MeasurementRepository.cs
    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly ApplicationDbContext _context;

        public MeasurementRepository(ApplicationDbContext context) => _context = context;

        public async Task<Measurement?> GetByIdAsync(long id)
            => await _context.Measurements.Include(m => m.Device).ThenInclude(d => d.House).FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Measurement>> GetByDeviceIdAsync(int deviceId, DateTime? from = null, DateTime? to = null, int limit = 100)
        {
            var query = _context.Measurements.Where(m => m.DeviceId == deviceId)
                .OrderByDescending(m => m.Timestamp);  // **Індекс DESC**
            if (from.HasValue) query = (IOrderedQueryable<Measurement>)query.Where(m => m.Timestamp >= from.Value);
            if (to.HasValue) query = (IOrderedQueryable<Measurement>)query.Where(m => m.Timestamp <= to.Value);
            return await query.Take(limit).ToListAsync();
        }

        public async Task<Measurement> CreateAsync(Measurement measurement)
        {
            _context.Measurements.Add(measurement);
            await _context.SaveChangesAsync();
            return measurement;
        }

        public async Task<Measurement?> UpdateAsync(long id, Measurement measurement)
        {
            var existing = await _context.Measurements.FindAsync(id);
            if (existing == null) return null;

            existing.Timestamp = measurement.Timestamp;
            existing.Value = measurement.Value;
            existing.Unit = measurement.Unit;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var measurement = await _context.Measurements.FindAsync(id);
            if (measurement == null) return false;

            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
