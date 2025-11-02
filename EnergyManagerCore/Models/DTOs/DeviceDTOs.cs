using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Models.DTOs
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";  // 'electricity_meter', ...
        public string Identifier { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class CreateDeviceDto
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Identifier { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
