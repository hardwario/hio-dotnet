using hio_dotnet.Common.Models.Common;
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
        [JsonPropertyName("analog_channels")]
        public List<AnalogChannel> AnalogChannels { get; set; } = new List<AnalogChannel>();

        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer> W1Thermometers { get; set; } = new List<W1_Thermometer>();
    }
}
