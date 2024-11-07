using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Dust
{
    public class ChesterDustCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("sps30")]
        public DustSensorData DustSensor { get; set; } = new DustSensorData();
    }
}
