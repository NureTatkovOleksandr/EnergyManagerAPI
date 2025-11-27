using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Models.DTOs
{
    public class HouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Address { get; set; }
        public int UserId { get; set; }
        public double? TotalArea { get; set; }  // м²
        public List<DeviceDto> Devices { get; set; } = new List<DeviceDto>();
    }

    public class CreateHouseDto
    {
        public string Name { get; set; } = "";
        public string? Address { get; set; }
        public double? TotalArea { get; set; }
    }
}
