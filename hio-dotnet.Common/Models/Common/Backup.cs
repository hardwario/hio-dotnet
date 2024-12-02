using hio_dotnet.Common.Models.DataSimulation;
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
        [SimulationAttribute(false, 23.5, 24.5, false)]
        [JsonPropertyName("line_voltage")]
        public double LineVoltage { get; set; } = 0.0;

        [SimulationAttribute(false, 3.0, 4.2, false)]
        [JsonPropertyName("batt_voltage")]
        public double BattVoltage { get; set; } = 0.0;

        [SimulationAttribute(true)]
        [JsonPropertyName("state")]
        public string? State { get; set; }

        [SimulationAttribute(true)]
        [JsonPropertyName("events")]
        public List<ChesterBaseEvent> Events { get; set; } = new List<ChesterBaseEvent>();
    }
}
