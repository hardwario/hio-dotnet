using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Thermometer
    {
        [SimulationAttribute(false, 25, 35)]
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.0;
    }
}
