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
    // Services/IDeviceService.cs
    public interface IDeviceService
    {
        Task<IEnumerable<DeviceDto>> GetByHouseIdAsync(int houseId, int userId);
        Task<DeviceDto?> GetByIdAsync(int id);
        Task<DeviceDto> CreateAsync(CreateDeviceDto dto, int houseId, int userId);
        Task<DeviceDto?> UpdateAsync(int id, DeviceDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }

    // Services/DeviceService.cs
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repo;
        private readonly IHouseService _houseService;  // Check ownership

        public DeviceService(IDeviceRepository repo, IHouseService houseService)
        {
            _repo = repo;
            _houseService = houseService;
        }

        public async Task<IEnumerable<DeviceDto>> GetByHouseIdAsync(int houseId, int userId)
        {
            var house = await _houseService.GetByIdAsync(houseId, userId);
            if (house == null) throw new UnauthorizedAccessException();
            var devices = await _repo.GetByHouseIdAsync(houseId);
            return devices.Select(d => new DeviceDto
            {
                Id = d.Id,
                HouseId = d.HouseId,
                Name = d.Name,
                Type = d.Type,
                Identifier = d.Identifier,
                IsActive = d.IsActive
            });
        }

        public async Task<DeviceDto?> GetByIdAsync(int id)
        {
            var device = await _repo.GetByIdAsync(id);
            if (device == null) return null;
            return new DeviceDto { /* map */ };
        }

        public async Task<DeviceDto> CreateAsync(CreateDeviceDto dto, int houseId, int userId)
        {
            var house = await _houseService.GetByIdAsync(houseId, userId);
            if (house == null) throw new UnauthorizedAccessException();
            var device = new Device
            {
                HouseId = houseId,
                Name = dto.Name,
                Type = dto.Type,
                Identifier = dto.Identifier,
                IsActive = dto.IsActive
            };
            var created = await _repo.CreateAsync(device);
            return new DeviceDto { /* map */ };
        }

        // Update/Delete аналогічно (check ownership)
        public async Task<DeviceDto?> UpdateAsync(int id, DeviceDto dto, int userId)
        {
            var device = await _repo.GetByIdAsync(id);
            if (device == null || device.House.UserId != userId)
                throw new UnauthorizedAccessException();

            device.Name = dto.Name;
            device.Type = dto.Type;
            device.Identifier = dto.Identifier;
            device.IsActive = dto.IsActive;

            var updated = await _repo.UpdateAsync(id,device);
            return new DeviceDto
            {
                Id = updated.Id,
                HouseId = updated.HouseId,
                Name = updated.Name,
                Type = updated.Type,
                Identifier = updated.Identifier,
                IsActive = updated.IsActive
            };
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var device = await _repo.GetByIdAsync(id);
            if (device == null || device.House.UserId != userId)
                throw new UnauthorizedAccessException();

            return await _repo.DeleteAsync(id);
        }

    }
}
