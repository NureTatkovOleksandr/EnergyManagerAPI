using EnergyManagerCore.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Models
{
    public class DashboardViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string Role { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public IEnumerable<HouseDto> Houses { get; set; } = new List<HouseDto>();
    }
}
