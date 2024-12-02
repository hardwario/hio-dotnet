using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Boiler
{
    public class BoilerCurrentLoopMeasurement : TimestampState
    {
        [JsonPropertyName("current")]
        public double Current { get; set; } = 0.0;

        [JsonPropertyName("current_avg")]
        public double Current_Avg { get; set; } = 0.0;

        [JsonPropertyName("current_max")]
        public double Current_Max { get; set; } = 0.0;

        [JsonPropertyName("current_mdn")]
        public double Current_Mdn { get; set; } = 0.0;

        [JsonPropertyName("current_min")]
        public double Current_Min { get; set; } = 0.0;

        [JsonPropertyName("sample_count")]
        public int SampleCount { get; set; } = 0;
    }
}
