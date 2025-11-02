
namespace EnergyManagerCore.Models
{
    // Models/User.cs
    // Models/DailySummary.cs
    public class DailySummary
    {
        public int Id { get; set; }

        public int HouseId { get; set; }  // FK

        public DateTime Date { get; set; } = DateTime.Today;

        public double TotalConsumption { get; set; } = 0;  // kWh

        public int? PeakHour { get; set; }  // 0-23

        public double Savings { get; set; } = 0;  // грн

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual House? House { get; set; }
    }
}
