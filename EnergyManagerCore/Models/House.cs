
namespace EnergyManagerCore.Models
{
    // Models/User.cs
    using System.ComponentModel.DataAnnotations;

    // Models/House.cs
    public class House
    {
        public int Id { get; set; }

        public int UserId { get; set; }  // FK

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        public string? Address { get; set; }

        public double? TotalArea { get; set; }  // м²

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual User? User { get; set; }
        public virtual ICollection<Device>? Devices { get; set; }
        public virtual ICollection<Scenario>? Scenarios { get; set; }
        public virtual ICollection<DailySummary>? DailySummaries { get; set; }
    }
}
