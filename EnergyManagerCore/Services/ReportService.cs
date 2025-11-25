using EnergyManagerCore.Models.DTOs;
using EnergyManagerCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Services
{

    public class ReportData
    {
        public List<(DeviceDto Device, List<MeasurementDto> Measurements, double Total)> DeviceMeasurements { get; set; } = new();
        public double GrandTotal { get; set; }
        public double MonthlyTotal { get; set; }
    }

    public interface IReportService
    {
        Task<ReportData> GenerateReportDataAsync(int userId);
    }
    
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<ReportData> GenerateReportDataAsync(int userId)
        {
            var devices = await _reportRepository.GetDevicesByUserIdAsync(userId);
            var deviceMeasurements = new List<(DeviceDto Device, List<MeasurementDto> Measurements, double Total)>();
            double grandTotal = 0;
            double monthlyTotal = 0;
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            foreach (var device in devices)
            {
                var measurements = await _reportRepository.GetMeasurementsByDeviceIdAsync(device.Id);
                var measurementDtos = measurements.Select(m => new MeasurementDto
                {
                    Id = m.Id,
                    DeviceId = m.DeviceId,
                    Timestamp = m.Timestamp,
                    Value = m.Value,
                    Unit = m.Unit
                }).ToList();

                // Sum only energy-related measurements (kWh for energy consumption; ignore others like °C)
                double total = measurements.Where(m => m.Unit == "kWh").Sum(m => m.Value);
                double deviceMonthly = measurements.Where(m => m.Unit == "kWh" && m.Timestamp >= oneMonthAgo).Sum(m => m.Value);

                var deviceDto = new DeviceDto
                {
                    Id = device.Id,
                    HouseId = device.HouseId,
                    Name = device.Name,
                    Type = device.Type,
                    Identifier = device.Identifier,
                    IsActive = device.IsActive
                };

                deviceMeasurements.Add((deviceDto, measurementDtos, total));
                grandTotal += total;
                monthlyTotal += deviceMonthly;
            }

            // Sort by total consumption descending
            deviceMeasurements = deviceMeasurements.OrderByDescending(d => d.Total).ToList();

            return new ReportData
            {
                DeviceMeasurements = deviceMeasurements,
                GrandTotal = grandTotal,
                MonthlyTotal = monthlyTotal
            };
        }
    }
}

