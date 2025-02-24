using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Hygro
    {
        [SimulationMeasurementAttribute(false, numberOfInsideItems: 3)]

        [JsonPropertyName("measurements")]
        public List<HygroData> Measurements { get; set; } = new List<HygroData>();
    }
}
