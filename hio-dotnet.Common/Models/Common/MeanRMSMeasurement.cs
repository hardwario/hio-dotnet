using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class MeanRMSMeasurement : TimestampState
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("mean_min")]
        public double MeanMin { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("mean_max")]
        public double MeanMax { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("mean_avg")]
        public double MeanAvg { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("mean_mdn")]
        public double MeanMdn { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_min")]
        public double RmsMin { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_max")]
        public double RmsMax { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_avg")]
        public double RmsAvg { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_mdn")]
        public double RmsMdn { get; set; } = 0.0;
    }
}
