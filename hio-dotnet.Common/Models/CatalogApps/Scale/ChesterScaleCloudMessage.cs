using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Scale
{
    public class ChesterScaleCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("weight")]
        [SimulationMeasurementAttribute(false, numberOfInsideItems:3)]
        public List<WeightMeasurement> Weights { get; set; } = new List<WeightMeasurement>();
    }
}
