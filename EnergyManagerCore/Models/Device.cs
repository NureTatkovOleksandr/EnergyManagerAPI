
namespace EnergyManagerCore.Models
{
    // Models/User.cs
    using System.ComponentModel.DataAnnotations;

    // Models/Device.cs
    public class Device
    {
        public int Id { get; set; }

        public int HouseId { get; set; }  // FK

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(50)]
        public string Type { get; set; } = "";  // 'electricity_meter', ...

        [Required, MaxLength(50)]
        public string Identifier { get; set; } = "";  // 'sim-001'

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual House? House { get; set; }
        public virtual ICollection<Measurement>? Measurements { get; set; }
    }
}
