using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.Wmbusmeters.Models
{
    public class WMBusMetersGasBase : WMBusMetersCommon
    {
        [JsonPropertyName("total_m3")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TotalM3 { get; set; }
        
        [JsonPropertyName("flow_m3h")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? FlowM3 { get; set; }
        
        [JsonPropertyName("target_m3")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TargetM3 { get; set; }
        
        [JsonPropertyName("target_datetime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TargetDateTime { get; set; }

        [JsonPropertyName("temperature_c")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TemperatureC { get; set; }
    }
}
