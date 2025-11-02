using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Models.DTOs
{
    public class MeasurementDto
    {
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; } = "";  // 'kWh', '°C'
    }

    public class CreateMeasurementDto
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double Value { get; set; }
        public string Unit { get; set; } = "";
    }
}
