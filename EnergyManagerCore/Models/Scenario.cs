
namespace EnergyManagerCore.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Scenario
    {
        public int Id { get; set; }

        public int HouseId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public string? Conditions { get; set; }

        public string? Actions { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual House? House { get; set; }
    }
}
