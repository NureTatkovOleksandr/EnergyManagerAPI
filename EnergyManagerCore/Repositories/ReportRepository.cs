using EnergyManagerCore.Data;
using EnergyManagerCore.Models;
using EnergyManagerCore.Models;
using global::EnergyManagerCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading.Tasks;
namespace EnergyManagerCore.Repositories
{
    public interface IReportRepository
    {
        Task<List<Device>> GetDevicesByUserIdAsync(int userId);
        Task<List<Measurement>> GetMeasurementsByDeviceIdAsync(int deviceId);
    }
    
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Device>> GetDevicesByUserIdAsync(int userId)
        {
            return await _context.Devices
                .Include(d => d.House)
                .Where(d => d.House.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Measurement>> GetMeasurementsByDeviceIdAsync(int deviceId)
        {
            return await _context.Measurements
                .Where(m => m.DeviceId == deviceId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
    }
}

