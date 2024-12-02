using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Range
{
    public class UltrasonicRanger
    {
        [SimulationMeasurementAttribute(false, 100.0, 200.0, true, false, 0.02)]
        [JsonPropertyName("distance")]
        public MeasurementGroup Distance { get; set; } = new MeasurementGroup();
    }
}
