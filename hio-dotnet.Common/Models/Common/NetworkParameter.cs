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
        [JsonPropertyName("eest")]
        public int Eest { get; set; } = 0;

        [JsonPropertyName("ecl")]
        public int Ecl { get; set; } = 0;

        [JsonPropertyName("rsrp")]
        public int Rsrp { get; set; } = -120;

        [JsonPropertyName("rsrq")]
        public int Rsrq { get; set; } = -20;

        [JsonPropertyName("snr")]
        public int Snr { get; set; } = 0;

        [JsonPropertyName("plmn")]
        public int Plmn { get; set; } = 0;

        [JsonPropertyName("cid")]
        public int Cid { get; set; } = 0;

        [JsonPropertyName("band")]
        public int Band { get; set; } = 0;

        [JsonPropertyName("earfcn")]
        public int Earfcn { get; set; } = 0;
    }
}
