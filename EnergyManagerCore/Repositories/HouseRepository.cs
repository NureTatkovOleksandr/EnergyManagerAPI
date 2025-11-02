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
    // Repositories/IHouseRepository.cs
    public interface IHouseRepository
    {
        Task<House?> GetByIdAsync(int id);
        Task<IEnumerable<House>> GetByUserIdAsync(int userId);
        Task<House> CreateAsync(House house);
        Task<House?> UpdateAsync(int id, House house);
        Task<bool> DeleteAsync(int id);
    }

    // Repositories/HouseRepository.cs
    public class HouseRepository : IHouseRepository
    {
        private readonly ApplicationDbContext _context;

        public HouseRepository(ApplicationDbContext context) => _context = context;

        public async Task<House?> GetByIdAsync(int id)
            => await _context.Houses.Include(h => h.User).FirstOrDefaultAsync(h => h.Id == id);

        public async Task<IEnumerable<House>> GetByUserIdAsync(int userId)
            => await _context.Houses.Where(h => h.UserId == userId).ToListAsync();

        public async Task<House> CreateAsync(House house)
        {
            _context.Houses.Add(house);
            await _context.SaveChangesAsync();
            return house;
        }

        public async Task<House?> UpdateAsync(int id, House house)
        {
            var existing = await _context.Houses.FindAsync(id);
            if (existing == null) return null;

            existing.Name = house.Name;
            existing.Address = house.Address;
            existing.TotalArea = house.TotalArea;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null) return false;

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();  // **CASCADE: devices/measurements DEL**
            return true;
        }
    }
}
