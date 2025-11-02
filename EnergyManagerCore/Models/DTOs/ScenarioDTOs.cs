using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Models.DTOs
{
    public class ScenarioDto
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Conditions { get; set; }  // JSON: '{"temp > 25": "turn_off_ac"}'
        public string? Actions { get; set; }     // JSON: '[{"device_id":1,"action":"set_temp","value":22}]'
        public bool IsActive { get; set; } = true;
    }

    public class CreateScenarioDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Conditions { get; set; }
        public string? Actions { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
