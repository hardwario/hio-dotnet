using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Dust
{
    public class DustSensorMeasurements
    {
        [SimulationMeasurementAttribute(false, numberOfInsideItems: 3)]
        [JsonPropertyName("measurements")]
        public List<DustSensorData> Measurements { get; set; } = new List<DustSensorData>();
    }
}
