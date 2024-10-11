using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Meteo
{
    public class BarometerMeasurements
    {
        [SimulationMeasurementAttribute(false, 200.0, 500.0, true, false, 0.02, 3)]
        [JsonPropertyName("pressure")]
        public MeasurementGroup Pressure { get; set; } = new MeasurementGroup();
    }
}
