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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? MeanMin { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("mean_max")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? MeanMax { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("mean_avg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? MeanAvg { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("mean_mdn")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? MeanMdn { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_min")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? RmsMin { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_max")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? RmsMax { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_avg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? RmsAvg { get; set; } = 0.0;

        [SimulationAttribute(false)]
        [JsonPropertyName("rms_mdn")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? RmsMdn { get; set; } = 0.0;
    }
}
