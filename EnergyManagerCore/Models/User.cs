
namespace EnergyManagerCore.Models
{
    // Models/User.cs
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50), Column(TypeName = "TEXT")]
        public string Username { get; set; } = "";

        [Required, MaxLength(100), Column(TypeName = "TEXT")]
        public string Email { get; set; } = "";

        [Required, MaxLength(255), Column(TypeName = "TEXT")]
        public string PasswordHash { get; set; } = "";

        [MaxLength(10), Column(TypeName = "TEXT")]
        public string Role { get; set; } = "user";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: Один User → Багато Houses
        public virtual ICollection<House>? Houses { get; set; }
    }
}
