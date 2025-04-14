using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class NetworkParameter
    {
        [SimulationAttribute(true)]
        [JsonPropertyName("eest")]
        public int? Eest { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("ecl")]
        public int? Ecl { get; set; } = 0;

        [SimulationAttribute(false, -120, 10, false, false, 0.01)]
        [JsonPropertyName("rsrp")]
        public int? Rsrp { get; set; } = -120;

        [SimulationAttribute(true)]
        [JsonPropertyName("rsrq")]
        public int? Rsrq { get; set; } = -20;

        [SimulationAttribute(true)]
        [JsonPropertyName("snr")]
        public int? Snr { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("plmn")]
        public int? Plmn { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("cid")]
        public int? Cid { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("band")]
        public int? Band { get; set; } = 0;

        [SimulationAttribute(true)]
        [JsonPropertyName("earfcn")]
        public int? Earfcn { get; set; } = 0;
    }
}
