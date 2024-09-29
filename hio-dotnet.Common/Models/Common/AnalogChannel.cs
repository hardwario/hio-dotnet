using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class AnalogChannel
    {
        [JsonPropertyName("channel")]
        public int Channel { get; set; } = 0;

        [JsonPropertyName("measurements")]
        public List<MeanRMSMeasurement> Measurements { get; set; } = new List<MeanRMSMeasurement>();
    }
}
