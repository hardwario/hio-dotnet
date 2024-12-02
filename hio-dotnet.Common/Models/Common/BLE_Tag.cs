using hio_dotnet.Common.Models.DataSimulation;
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
        [SimulationAttribute(true)]
        [JsonPropertyName("addr")]
        public string Addr { get; set; } = Defaults.Unknown;

        [SimulationAttribute(false, -120.0, 10.0)]
        [JsonPropertyName("rssi")]
        public int Rssi { get; set; } = 0;

        [SimulationAttribute(false, 2.0, 3.2, true, false, 0.01)]
        [JsonPropertyName("voltage")]
        public double Voltage { get; set; } = 0.0;

        [SimulationMeasurementAttribute(false, 0.0, 100.0, true, false, 0.02)]
        [JsonPropertyName("humidity")]
        public MeasurementGroup Humidity { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 25.0, 30.0, true, false, 0.03)]
        [JsonPropertyName("temperature")]
        public MeasurementGroup Temperature { get; set; } = new MeasurementGroup();
    }
}
