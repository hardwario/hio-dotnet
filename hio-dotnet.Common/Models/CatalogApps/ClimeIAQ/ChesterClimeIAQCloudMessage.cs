using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.ClimeIAQ
{
    public class ChesterClimeIAQCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("iaq_sensor")]
        public IAQ_Sensor IAQ_Sensor { get; set; } = new IAQ_Sensor();

        [JsonPropertyName("hygrometer")]
        public Hygrometer? Hygrometer { get; set; }

        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer>? W1_Thermometers { get; set; }

        [JsonPropertyName("rtd_thermometer")]
        public List<RTD_Thermometer>? RTD_Thermometers { get; set; }

        [JsonPropertyName("ble_tags")]
        public List<BLE_Tag>? BLE_Tags { get; set; }
    }
}
