using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class BLE_Tag
    {
        [JsonPropertyName("addr")]
        public string Addr { get; set; } = Defaults.Unknown;

        [JsonPropertyName("rssi")]
        public int Rssi { get; set; } = 0;

        [JsonPropertyName("voltage")]
        public double Voltage { get; set; } = 0.0;

        [JsonPropertyName("humidity")]
        public MeasurementGroup Humidity { get; set; } = new MeasurementGroup();

        [JsonPropertyName("temperature")]
        public MeasurementGroup Temperature { get; set; } = new MeasurementGroup();
    }
}
