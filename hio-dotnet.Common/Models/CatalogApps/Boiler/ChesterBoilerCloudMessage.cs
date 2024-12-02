using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Boiler
{
    public class ChesterBoilerCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("current_4_20ma")]
        public BoilerCurrentLoopMeasurement CurrentLoop { get; set; } = new BoilerCurrentLoopMeasurement();

        [JsonPropertyName("input_1")]
        public BoilerInputState Input_1 { get; set; } = new BoilerInputState();

        [JsonPropertyName("input_2")]
        public BoilerInputState Input_2 { get; set; } = new BoilerInputState();

        [JsonPropertyName("line_status")]
        public BoilerLineStatus LineStatus { get; set; } = new BoilerLineStatus();

        [JsonPropertyName("voltage_0_10v")]
        public BoilerVoltageMeasurement VoltageInput { get; set; } = new BoilerVoltageMeasurement();

        [JsonPropertyName("w1_thermometers")]
        public List<BoilerW1ThermometerMeasurement> W1Thermometers { get; set; } = new List<BoilerW1ThermometerMeasurement>();

    }
}
