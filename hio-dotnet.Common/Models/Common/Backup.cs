using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Backup
    {
        [JsonPropertyName("line_voltage")]
        public double LineVoltage { get; set; } = 0.0;

        [JsonPropertyName("batt_voltage")]
        public double BattVoltage { get; set; } = 0.0;

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("events")]
        public List<ChesterEvent> Events { get; set; } = new List<ChesterEvent>();
    }
}
