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
    // Services/IHouseService.cs
    public interface IHouseService
    {
        Task<IEnumerable<HouseDto>> GetMyHousesAsync(int userId);
        Task<IEnumerable<DeviceDto>> GetHomeDevices(int houseId);
        Task<HouseDto?> GetByIdAsync(int id, int userId);
        Task<HouseDto> CreateAsync(CreateHouseDto dto, int userId);
        Task<HouseDto?> UpdateAsync(int id, HouseDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }

    // Services/HouseService.cs
    public class HouseService : IHouseService
    {
        private readonly IHouseRepository _repo;

        public HouseService(IHouseRepository repo) => _repo = repo;

        public async Task<IEnumerable<HouseDto>> GetMyHousesAsync(int userId)
        {
            var houses = await _repo.GetByUserIdAsync(userId);
            return houses.Select(h => new HouseDto
            {
                Id = h.Id,
                Name = h.Name,
                Address = h.Address,
                TotalArea = h.TotalArea
            });
        }
        public async Task<IEnumerable<DeviceDto>> GetHomeDevices(int houseId)
        {
            var meshs = await _repo.GetHomeDevices(houseId);
            return meshs.Select(m => new DeviceDto()
            {
                Id = m.Id,
                Name = m.Name,
                HouseId = houseId,
                Type = m.Type,
                Identifier = m.Identifier,
                IsActive = m.IsActive
            });
        }
        public async Task<HouseDto?> GetByIdAsync(int id, int userId)
        {
            var house = await _repo.GetByIdAsync(id);
            if (house == null || house.UserId != userId) return null;
            return new HouseDto { Id = house.Id, Name = house.Name, Address = house.Address, TotalArea = house.TotalArea, UserId = house.UserId };
        }

        public async Task<HouseDto> CreateAsync(CreateHouseDto dto, int userId)
        {
            var house = new House
            {
                UserId = userId,
                Name = dto.Name,
                Address = dto.Address,
                TotalArea = dto.TotalArea
            };
            var created = await _repo.CreateAsync(house);
            return new HouseDto { Id = created.Id, Name = created.Name, Address = created.Address, TotalArea = created.TotalArea };
        }

        public async Task<HouseDto?> UpdateAsync(int id, HouseDto dto, int userId)
        {
            var house = new House { Id = id, Name = dto.Name, Address = dto.Address, TotalArea = dto.TotalArea };
            var updated = await _repo.UpdateAsync(id, house);
            return updated != null ? new HouseDto { Id = updated.Id, Name = updated.Name, Address = updated.Address, TotalArea = updated.TotalArea } : null;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var house = await _repo.GetByIdAsync(id);
            if (house == null || house.UserId != userId) return false;
            return await _repo.DeleteAsync(id);
        }
    }
}
