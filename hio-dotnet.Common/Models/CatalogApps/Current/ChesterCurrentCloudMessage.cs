using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Current
{
    public class ChesterCurrentCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationMeasurementAttribute(false, 400, 700, true, false, 0, 4)]
        [JsonPropertyName("analog_channels")]
        public List<AnalogChannel> AnalogChannels { get; set; } = new List<AnalogChannel>();

        [SimulationMeasurementAttribute(false, 25, 35, true, false, 0, 2)]
        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer> W1Thermometers { get; set; } = new List<W1_Thermometer>();
    }
}
