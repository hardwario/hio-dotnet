using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Boiler
{
    public class BoilerVoltageMeasurement : TimestampState
    {
        [JsonPropertyName("voltage")]
        public double Voltage { get; set; } = 0.0;

        [JsonPropertyName("voltage_avg")]
        public double Voltage_Avg { get; set; } = 0.0;

        [JsonPropertyName("voltage_max")]
        public double Voltage_Max { get; set; } = 0.0;

        [JsonPropertyName("voltage_mdn")]
        public double Voltage_Mdn { get; set; } = 0.0;

        [JsonPropertyName("voltage_min")]
        public double Voltage_Min { get; set; } = 0.0;

        [JsonPropertyName("sample_count")]
        public int SampleCount { get; set; } = 0;
    }
}
