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
    // Services/IMeasurementService.cs
    public interface IMeasurementService
    {
        Task<IEnumerable<MeasurementDto>> GetByDeviceIdAsync(int deviceId, int userId, DateTime? from = null, DateTime? to = null);
        Task<MeasurementDto?> GetByIdAsync(long id, int userId);
        Task<MeasurementDto> CreateAsync(CreateMeasurementDto dto, int deviceId);
        Task<MeasurementDto?> UpdateAsync(long id, MeasurementDto dto, int userId);
        Task<bool> DeleteAsync(long id, int userId);
    }

    // Services/MeasurementService.cs
    public class MeasurementService : IMeasurementService
    {
        private readonly IMeasurementRepository _repo;
        private readonly IDeviceService _deviceService;  // **Зв'язок: check device.house.user**

        public MeasurementService(IMeasurementRepository repo, IDeviceService deviceService)
        {
            _repo = repo;
            _deviceService = deviceService;
        }

        public async Task<IEnumerable<MeasurementDto>> GetByDeviceIdAsync(int deviceId, int userId, DateTime? from = null, DateTime? to = null)
        {
            var device = await _deviceService.GetByIdAsync(deviceId);
            if (device == null) throw new UnauthorizedAccessException();
            var measurements = await _repo.GetByDeviceIdAsync(deviceId, from, to);
            return measurements.Select(m => new MeasurementDto
            {
                Id = m.Id,
                DeviceId = m.DeviceId,
                Timestamp = m.Timestamp,
                Value = m.Value,
                Unit = m.Unit
            });
        }

        public async Task<MeasurementDto?> GetByIdAsync(long id, int userId)
        {
            var measurement = await _repo.GetByIdAsync(id);
            if (measurement == null || measurement.Device.House.UserId != userId) return null;
            return new MeasurementDto { /* map */ };
        }

        public async Task<MeasurementDto> CreateAsync(CreateMeasurementDto dto, int deviceId)
        {
            var device = await _deviceService.GetByIdAsync(deviceId);
            if (device == null) throw new NullReferenceException("Device is null.");
            var measurement = new Measurement
            {
                DeviceId = deviceId,
                Timestamp = dto.Timestamp,
                Value = dto.Value,
                Unit = dto.Unit
            };
            var created = await _repo.CreateAsync(measurement);
            return new MeasurementDto { /* map */ };

        }

        public async Task<MeasurementDto?> UpdateAsync(long id, MeasurementDto dto, int userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.Device.House.UserId != userId)
                return null;

            existing.Timestamp = dto.Timestamp;
            existing.Value = dto.Value;
            existing.Unit = dto.Unit;

            var updated = await _repo.UpdateAsync(id, existing);
            return new MeasurementDto
            {
                Id = updated.Id,
                DeviceId = updated.DeviceId,
                Timestamp = updated.Timestamp,
                Value = updated.Value,
                Unit = updated.Unit
            };
        }

        public async Task<bool> DeleteAsync(long id, int userId)
        {
            var measurement = await _repo.GetByIdAsync(id);
            if (measurement == null || measurement.Device.House.UserId != userId)
                return false;

            return await _repo.DeleteAsync(id);
        }

    }
}
