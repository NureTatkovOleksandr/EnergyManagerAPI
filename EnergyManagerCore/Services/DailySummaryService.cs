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
    // Services/IDailySummaryService.cs
    public interface IDailySummaryService
    {
        Task<IEnumerable<DailySummaryDto>> GetByHouseIdAsync(int houseId, int userId, DateTime? from = null, DateTime? to = null);
        Task<DailySummaryDto?> GetByIdAsync(int id, int userId);
        Task<DailySummaryDto> CreateAsync(CreateDailySummaryDto dto, int houseId, int userId);
        Task<DailySummaryDto?> UpdateAsync(int id, DailySummaryDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }

    // Services/DailySummaryService.cs
    public class DailySummaryService : IDailySummaryService
    {
        private readonly IDailySummaryRepository _repo;
        private readonly IHouseService _houseService;

        public DailySummaryService(IDailySummaryRepository repo, IHouseService houseService)
        {
            _repo = repo;
            _houseService = houseService;
        }

        public async Task<IEnumerable<DailySummaryDto>> GetByHouseIdAsync(int houseId, int userId, DateTime? from = null, DateTime? to = null)
        {
            var house = await _houseService.GetByIdAsync(houseId, userId);
            if (house == null) throw new UnauthorizedAccessException();
            var summaries = await _repo.GetByHouseIdAsync(houseId, from, to);
            return summaries.Select(ds => new DailySummaryDto
            {
                Id = ds.Id,
                HouseId = ds.HouseId,
                Date = ds.Date,
                TotalConsumption = ds.TotalConsumption,
                PeakHour = ds.PeakHour,
                Savings = ds.Savings
            });
        }

        public async Task<DailySummaryDto?> GetByIdAsync(int id, int userId)
        {
            var summary = await _repo.GetByIdAsync(id);
            if (summary == null || summary.House.UserId != userId) return null;

            return new DailySummaryDto
            {
                Id = summary.Id,
                HouseId = summary.HouseId,
                Date = summary.Date,
                TotalConsumption = summary.TotalConsumption,
                PeakHour = summary.PeakHour,
                Savings = summary.Savings
            };
        }

        public async Task<DailySummaryDto> CreateAsync(CreateDailySummaryDto dto, int houseId, int userId)
        {
            var house = await _houseService.GetByIdAsync(houseId, userId);
            if (house == null) throw new UnauthorizedAccessException();

            var summary = new DailySummary
            {
                HouseId = houseId,
                Date = dto.Date,
                TotalConsumption = dto.TotalConsumption,
                PeakHour = dto.PeakHour,
                Savings = dto.Savings
            };

            var created = await _repo.CreateAsync(summary);
            return new DailySummaryDto
            {
                Id = created.Id,
                HouseId = created.HouseId,
                Date = created.Date,
                TotalConsumption = created.TotalConsumption,
                PeakHour = created.PeakHour,
                Savings = created.Savings
            };
        }

        public async Task<DailySummaryDto?> UpdateAsync(int id, DailySummaryDto dto, int userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.House.UserId != userId) return null;

            existing.Date = dto.Date;
            existing.TotalConsumption = dto.TotalConsumption;
            existing.PeakHour = dto.PeakHour;
            existing.Savings = dto.Savings;

            var updated = await _repo.UpdateAsync(id, existing);
            return new DailySummaryDto
            {
                Id = updated.Id,
                HouseId = updated.HouseId,
                Date = updated.Date,
                TotalConsumption = updated.TotalConsumption,
                PeakHour = updated.PeakHour,
                Savings = updated.Savings
            };
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.House.UserId != userId) return false;

            return await _repo.DeleteAsync(id);
        }

    }
}