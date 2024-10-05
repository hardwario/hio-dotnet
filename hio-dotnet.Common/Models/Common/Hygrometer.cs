using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Hygrometer
    {
        [SimulationMeasurementAttribute(true)]
        [JsonPropertyName("temperature")]
        public Temperature Temperature { get; set; } = new Temperature();

        [SimulationMeasurementAttribute(false, 0.0, 100.0, true, false, 0.02)]
        [JsonPropertyName("humidity")]
        public MeasurementGroup Humidity { get; set; } = new MeasurementGroup();
    }
}
