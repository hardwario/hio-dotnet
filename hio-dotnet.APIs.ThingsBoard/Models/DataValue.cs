using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class DataValue
    {
        public DateTime Timestamp { get; set; } = DateTime.MinValue;
        public double Value { get; set; } = 0;
    }
}
