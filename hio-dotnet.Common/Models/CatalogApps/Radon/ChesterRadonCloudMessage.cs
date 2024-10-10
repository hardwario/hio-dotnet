using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Radon
{
    public class ChesterRadonCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("radon_probe")]
        public RadonProbeData RadonProbe { get; set; } = new RadonProbeData();
    }
}
