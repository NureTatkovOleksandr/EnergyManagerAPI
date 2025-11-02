using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Models.DTOs
{
    public class DailySummaryDto
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public DateTime Date { get; set; }
        public double TotalConsumption { get; set; }  // kWh
        public int? PeakHour { get; set; }  // 0-23
        public double Savings { get; set; }  // грн
    }

    public class CreateDailySummaryDto
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public double TotalConsumption { get; set; } = 0;
        public int? PeakHour { get; set; }
        public double Savings { get; set; } = 0;
    }
}
