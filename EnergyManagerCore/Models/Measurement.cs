
namespace EnergyManagerCore.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Measurement
    {
        public long Id { get; set; } 

        public int DeviceId { get; set; }  

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public double Value { get; set; }

        [Required, MaxLength(10)]
        public string Unit { get; set; } = "";

        // Navigation
        public virtual Device? Device { get; set; }
    }
}
