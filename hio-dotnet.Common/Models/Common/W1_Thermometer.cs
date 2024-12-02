using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class W1_Thermometer
    {
        [JsonPropertyName("serial_number")]
        public long SerialNumber { get; set; } = 0;

        [JsonPropertyName("measurements")]
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
