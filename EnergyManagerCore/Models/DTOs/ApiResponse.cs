using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Models.DTOs
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public T User { get; set; }
    }

}
