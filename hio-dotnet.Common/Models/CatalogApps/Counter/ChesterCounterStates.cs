using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Counter
{
    public class ChesterCounterStates
    {
        [JsonPropertyName("channel_1_total")]
        public int? Channel1Total { get; set; }

        [JsonPropertyName("channel_1_delta")]
        public int? Channel1Delta { get; set; }

        [JsonPropertyName("channel_2_total")]
        public int? Channel2Total { get; set; }

        [JsonPropertyName("channel_2_delta")]
        public int? Channel2Delta { get; set; }

        [JsonPropertyName("channel_3_total")]
        public int? Channel3Total { get; set; }

        [JsonPropertyName("channel_3_delta")]
        public int? Channel3Delta { get; set; }

        [JsonPropertyName("channel_4_total")]
        public int? Channel4Total { get; set; }

        [JsonPropertyName("channel_4_delta")]
        public int? Channel4Delta { get; set; }

        [JsonPropertyName("channel_5_total")]
        public int? Channel5Total { get; set; }

        [JsonPropertyName("channel_5_delta")]
        public int? Channel5Delta { get; set; }

        [JsonPropertyName("channel_6_total")]
        public int? Channel6Total { get; set; }

        [JsonPropertyName("channel_6_delta")]
        public int? Channel6Delta { get; set; }

        [JsonPropertyName("channel_7_total")]
        public int? Channel7Total { get; set; }

        [JsonPropertyName("channel_7_delta")]
        public int? Channel7Delta { get; set; }

        [JsonPropertyName("channel_8_total")]
        public int? Channel8Total { get; set; }

        [JsonPropertyName("channel_8_delta")]
        public int? Channel8Delta { get; set; }
    }
}
