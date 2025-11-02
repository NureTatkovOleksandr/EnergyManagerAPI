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
    // Repositories/IDeviceRepository.cs
    public interface IDeviceRepository
    {
        Task<Device?> GetByIdAsync(int id);
        Task<IEnumerable<Device>> GetByHouseIdAsync(int houseId);
        Task<Device> CreateAsync(Device device);
        Task<Device?> UpdateAsync(int id, Device device);
        Task<bool> DeleteAsync(int id);
    }

    // Repositories/DeviceRepository.cs
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ApplicationDbContext _context;

        public DeviceRepository(ApplicationDbContext context) => _context = context;

        public async Task<Device?> GetByIdAsync(int id)
            => await _context.Devices.Include(d => d.House).FirstOrDefaultAsync(d => d.Id == id);

        public async Task<IEnumerable<Device>> GetByHouseIdAsync(int houseId)
            => await _context.Devices.Where(d => d.HouseId == houseId).ToListAsync();

        public async Task<Device> CreateAsync(Device device)
        {
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<Device?> UpdateAsync(int id, Device device)
        {
            var existing = await _context.Devices.FindAsync(id);
            if (existing == null) return null;

            existing.Name = device.Name;
            existing.Type = device.Type;
            existing.Identifier = device.Identifier;
            existing.IsActive = device.IsActive;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null) return false;

            _context.Devices.Remove(device);  // **CASCADE measurements**
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
