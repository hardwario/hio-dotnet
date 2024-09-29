using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Boiler
{
    public class BoilerW1ThermometerMeasurement : TimestampState
    {
        [JsonPropertyName("serial_number")]
        public long SerialNumber { get; set; } = 0;

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.0;

        [JsonPropertyName("temperature_avg")]
        public double Temperature_Avg { get; set; } = 0.0;

        [JsonPropertyName("temperature_max")]
        public double Temperature_Max { get; set; } = 0.0;

        [JsonPropertyName("temperature_mdn")]
        public double Temperature_Mdn { get; set; } = 0.0;

        [JsonPropertyName("temperature_min")]
        public double Temperature_Min { get; set; } = 0.0;

        [JsonPropertyName("sample_count")]
        public int SampleCount { get; set; } = 0;
    }
}
