using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Range
{
    public class ChesterRangeCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("ultrasonic_ranger")]
        public UltrasonicRanger UltrasonicRanger { get; set; } = new UltrasonicRanger();

        [JsonPropertyName("hygrometer")]
        public Hygrometer Hygrometer { get; set; } = new Hygrometer();

        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer> W1Thermometers { get; set; } = new List<W1_Thermometer>();
    }
}
