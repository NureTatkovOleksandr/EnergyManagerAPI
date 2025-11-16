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
        // В інтерфейсі IDeviceService
        Task<List<MeasurementDto>?> GetMeasurementsByDeviceIdAsync(int deviceId, int userId);
        Task<double?> GetTotalMeasurementValueAsync(int deviceId, int userId);
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
        private readonly IMeasurementRepository _measurementRepository;
        public DeviceService(IDeviceRepository repo, IHouseService houseService, IMeasurementRepository measurementRepository)
        {
            _repo = repo;
            _houseService = houseService;
            _measurementRepository = measurementRepository;
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
        // В DeviceService (приклад реалізації в сервісі)
        // DeviceService.cs (додай/заміни ці методи)

        public async Task<List<MeasurementDto>?> GetMeasurementsByDeviceIdAsync(int deviceId, int userId)
        {
            // Перевіряємо, чи пристрій існує і належить користувачу
            var device = await _repo.GetByIdAsync(deviceId);
            if (device == null || device.House?.UserId != userId)
                return null;

            // Отримуємо ентіті Measurement з репозиторію
            var entities = await _measurementRepository.GetByDeviceIdAsync(deviceId);

            // Мапимо на DTO (вручну – найпростіше для тестового проекту)
            var dtos = entities.Select(m => new MeasurementDto
            {
                Id = m.Id,
                DeviceId = m.DeviceId,
                Timestamp = m.Timestamp,
                Value = m.Value,
                Unit = m.Unit
            }).OrderByDescending(x => x.Timestamp) // зручно для фронту
              .ToList();

            return dtos;
        }

        public async Task<double?> GetTotalMeasurementValueAsync(int deviceId, int userId)
        {
            var device = await _repo.GetByIdAsync(deviceId);
            if (device == null || device.House?.UserId != userId)
                return null;

            var entities = await _measurementRepository.GetByDeviceIdAsync(deviceId);

            // Якщо вимірювань немає – повертаємо 0.0 (не null)
            if (!entities.Any())
                return 0.0;

            // Просто сумуємо Value (як просив – без урахування unit, бо це тестовий проект)
            return entities.Sum(m => m.Value);
        }
    }
}
