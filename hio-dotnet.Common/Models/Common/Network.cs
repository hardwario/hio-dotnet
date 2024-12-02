using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Network
    {
        [SimulationAttribute(true)]
        [JsonPropertyName("imei")]
        public long Imei { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("imsi")]
        public long Imsi { get; set; } = 0;

        [SimulationAttribute(false)]
        [JsonPropertyName("parameter")]
        public NetworkParameter Parameter { get; set; } = new NetworkParameter();
    }
}
